using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectsManagment.Models;

namespace ProjectsManagment.Controllers
{
    [Authorize]
    public class PartitionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Partition/
        public ActionResult Index(int? id)
        {
            if(id==null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ViewBag.Id = id;
            var stage = db.Stages.Where(s => s.Id == id).Select(p => p).First();
            if (stage != null)
                ViewBag.StageName = stage.Name;//db.Stages.Where(s => s.Id == id).Select(p => p.Name).First();
            else
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);           
            ViewBag.ProjectName = db.Projects.Where(s => s.Id == stage.ProjectId).Select(p => p.Name).First();

            var partitions = db.Partitions.Where(p => p.StageId == id).Select(s => s);
            FillViewBag((int)id);
            ViewBag.My = false;
            return View(partitions.ToList());
        }

        public ActionResult Show(Partition partition)
        {
            if (partition == null)
                return HttpNotFound();
            else
                return PartialView("PartitionInfo", partition);
        }

        public ActionResult ShowDetails(Partition partition)
        {
            if (partition == null)
                return HttpNotFound();
            else
                return PartialView("PartitionDetails", partition);
        }

        public ActionResult MyPartitions()
        {
            ApplicationUser iam = db.Users.Where(u => u.UserName == User.Identity.Name).Select(user => user).First();
            var parts = db.Partitions.Where(t => t.Manager.Id == iam.Id).Select(task => task);
            ViewBag.My = true;
            return View("Index", parts);
        }

        public static void SaveDB(ApplicationDbContext context)
        {
            try
            {
                context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Главный инженер проекта, Руководитель направления, Администратор")]
        public ActionResult ShowInfoForCancel(int? id, bool details)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Partition partition = db.Partitions.Find((int)id);
            if (partition == null)
                return HttpNotFound();
            Comment comment = new Comment();
            comment.ItemId = (int)id;
            ViewBag.Error = "";
            ViewBag.Table = "Partition";
            ViewBag.Details = details;
            return PartialView("AddCancelComment", comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Главный инженер проекта, Руководитель направления, Администратор")]
        public ActionResult MakeFinish(int? id, bool details)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Partition partition = db.Partitions.Find((int)id);
            if (partition == null)
                return HttpNotFound();
            ApplicationUser user = db.Users.Where(us => us.UserName == User.Identity.Name).Select(u => u).First();
            Event e;
            if (partition.Manager != null && user.UserName == partition.Manager.UserName)
            {
                partition.IsPreFinish = true;
                partition.State = ProjectStateController.GetState(db, "ожидает проверки"); //db.TasksStates.Where(ts => ts.Name.ToLower() == "ожидает проверки").Select(s => s).First();
                partition.StateId = partition.State.Id;
                e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "отметил(а) как завершенное"), user, DependencyTable.Partition, partition);
            }
            else
            {
                partition.IsPreFinish = false;
                partition.IsFinish = true;
                partition.State = ProjectStateController.GetState(db, "завершено"); //db.TasksStates.Where(ts => ts.Name.ToLower() == "завершено").Select(s => s).First();
                partition.StateId = partition.State.Id;
                e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "подтвердил(а) завершение"), user, DependencyTable.Partition, partition);
            }
           
            partition.Creator = db.Users.Find(partition.Creator.Id);
            db.Entry(partition).State = EntityState.Modified;
            EventController.CreateEventForUsers(db, e, GetUsersForPartitionEvent(partition));
            SaveDB(db);
            //db.SaveChanges();
            if (!details)
                return Show(partition);
            else
                return ShowDetails(partition);
        }

        private IEnumerable<ApplicationUser> GetUsersForPartitionEvent(Partition partition)
        {
            List<ApplicationUser> result = new List<ApplicationUser>();
            if (partition.Manager != null)
                result.Add(partition.Manager);
            if (partition.Stage.Executor != null && result.Find(user => user.Id == partition.Stage.Executor.Id) == null)
                result.Add(partition.Stage.Executor);
            if (partition.Stage.Project.Manager != null && result.Find(user => user.Id == partition.Stage.Project.Manager.Id) == null)
                result.Add(partition.Stage.Project.Manager);
            var directors = ProjectController.GetUsersByRoles(db, new List<string>() { "директор", "главный инженер проекта" });
            foreach (var user in directors)
                if (result.Find(u => u.Id == user.Id) == null)
                    result.Add(user);
            return result;
        }

        public Partition CF(Comment comment)
        {
            
            Partition partition = db.Partitions.Find(comment.ItemId);
            if (partition == null)
                return null;
            partition.IsPreFinish = false;
            partition.IsFinish = false;
            partition.State = ProjectStateController.GetState(db, "доработка");//db.TasksStates.Where(ts => ts.Name.ToLower() == "доработка").Select(s => s).First();
            partition.StateId = partition.State.Id;
            partition.Creator = db.Users.Find(partition.Creator.Id);
            ApplicationUser user = db.Users.Where(us => us.UserName == User.Identity.Name).Select(u => u).First();
            Event e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "отправил(а) на доработку"), user, DependencyTable.Partition, partition);
            var users = GetUsersForPartitionEvent(partition);
            EventController.CreateEventForUsers(db, e, users);
            db.Entry(partition).State = EntityState.Modified;
            comment = CommentController.AddComment(db, comment.Text, "разделы", comment.ItemId, user, true);
            e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "оставил(а) замечание"), user, DependencyTable.Comment, comment);
            EventController.CreateEventForUsers(db, e, users);
            SaveDB(db);
            
            return partition;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Главный инженер проекта, Руководитель направления, Администратор")]
        public ActionResult CancelFinish(Comment comment)
        {
            if (string.IsNullOrEmpty(comment.Text))
            {
                ViewBag.Error = "Необходимо ввести замечание";
                ViewBag.Table = "Partition";
                return PartialView(comment);
            }
            else
            {
                return Show(CF(comment));
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Главный инженер проекта, Руководитель направления, Администратор")]
        public ActionResult CancelFinish2(Comment comment)
        {
            if (string.IsNullOrEmpty(comment.Text))
            {
                ViewBag.Error = "Необходимо ввести замечание";
                ViewBag.Table = "Partition";
                return PartialView(comment);
            }
            else
            {                
                return View("Details", CF(comment));
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakePreFinish(int? id, bool details)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Partition partition = db.Partitions.Find((int)id);
            if (partition == null)
                return HttpNotFound();
            partition.IsPreFinish = true;
            partition.State = ProjectStateController.GetState(db, "ожидает проверки");//db.TasksStates.Where(ts => ts.Name.ToLower() == "ожидает проверки").Select(s => s).First();
            partition.StateId = partition.State.Id;
            partition.Creator = db.Users.Find(partition.Creator.Id);
            db.Entry(partition).State = EntityState.Modified;
            SaveDB(db);
            if (!details)
                return Show(partition);
            else
                return ShowDetails(partition);
        }

        private void FillViewBag(int id)
        {
            Stage part = db.Stages.Find(id);
            ViewBag.ProjectName = part.Project.Name;
            ViewBag.ProjectId = part.ProjectId;
            ViewBag.StageName = part.Name;
            ViewBag.StageId = id;
        }

        public ActionResult GetComments(int partitionId, int count)
        {
            int tableId = TableTypesController.GetTableTypeId(db, "разделы");//TableTypesController.GetTableTypeId(db, "разделы");//db.TableTypes.Where(t => t.TableName.Trim().ToLower() == "разделы").Select(table => table.Id).First();
            return PartialView("CommentSummary", CommentController.GetCommentsFor(db, tableId, partitionId, count));
        }

        // GET: /Partition/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Partition partition = db.Partitions.Find(id);
            if (partition == null)
            {
                return HttpNotFound();
            }
            ViewBag.TableId = TableTypesController.GetTableTypeId(db, "разделы"); //db.TableTypes.Where(t => t.TableName.Trim().ToLower() == "разделы").Select(table => table.Id).First();
            FillViewBag(partition.StageId);
            return View(partition);
        }

        // GET: /Partition/Create
        [Authorize(Roles = "Директор, Главный инженер проекта, Руководитель направления, Администратор")]
        public ActionResult Create(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            PartitionCreateView pcv = new PartitionCreateView();
            ViewBag.DeadLine = DateTime.Now.AddDays(1);
            pcv.ParentId = (int)id;
            ViewBag.pId = id;
            ViewData["UserId"] = ProjectController.GetSelectedListUsersByRoles(db, new List<string>() { "исполнитель", "руководитель направления" });
            //ViewBag.Managers = selectList;//new SelectList(db.Users, "Id", "UserName");
            ViewBag.SelectedItem = string.Empty;
            FillViewBag((int)id);
            return View(pcv);
        }

        // POST: /Partition/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Главный инженер проекта, Руководитель направления, Администратор")]
        public ActionResult Create(PartitionCreateView pcv)//Partition partition)
        {
            if (pcv.ParentId <= 0 || db.Stages.Where(stage => stage.Id == pcv.ParentId).Select(s => s).Count() == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);           
            
            if (ModelState.IsValid)
            {
                Partition partition = new Partition();
                partition.CreationDate = DateTime.Now;
                partition.DeadLine = pcv.DeadLine;
                partition.Name = pcv.Name;
                partition.Description = pcv.Description;
                if (string.IsNullOrEmpty(pcv.UserId))
                {
                    partition.State = ProjectController.GetTaskState(db, "отсутствует менеджер");
                    partition.Manager = null;
                }
                else
                {
                    partition.State = ProjectController.GetTaskState(db, "в работе");
                    partition.Manager = db.Users.Where(u => u.Id == pcv.UserId).Select(user => user).First();
                }
                partition.StateId = partition.State.Id;
                partition.Creator = db.Users.Where(u => u.UserName == User.Identity.Name).Select(n => n).First();
                partition.Stage = db.Stages.Where(p=>p.Id == pcv.ParentId).Select(s => s).First();
                partition.StageId = partition.Stage.Id;
                //partition.State = db.TasksStates.Where(p=>p.Id == partition.StageId).Select(s => s).First();
                db.Partitions.Add(partition);
                Event e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "создал(а)"), partition.Creator, DependencyTable.Partition, partition);
                EventController.CreateEventForUsers(db, e, GetUsersForPartitionEvent(partition));
                ProjectController.SaveDB(db);
                FillViewBag(pcv.ParentId);
                return RedirectToAction("Index", new { id = pcv.ParentId });
            }
            ViewBag.pId = pcv.ParentId;
            ViewBag.DeadLine = pcv.DeadLine;
            
            ViewData["UserId"] = ProjectController.GetSelectedListUsersByRoles(db, new List<string>() { "исполнитель", "руководитель направления" }, pcv.UserId);
            //selectList.Where(i => i.Value == pcv.UserId).Select(item => item).First().Selected = true;
            FillViewBag(pcv.ParentId);
            return View(pcv);
        }

        // GET: /Partition/Edit/5
        [Authorize(Roles = "Директор, Главный инженер проекта, Руководитель направления, Администратор")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Partition partition = db.Partitions.Find(id);
            if (partition == null)
            {
                return HttpNotFound();
            }
            PartitionCreateView pcv = new PartitionCreateView();
            pcv.DeadLine = partition.DeadLine;
            pcv.Description = partition.Description;
            pcv.Name = partition.Name;
            pcv.Id = partition.Id;
            pcv.IsFinish = partition.IsFinish;
            pcv.IsPreFinish = partition.IsPreFinish;
            pcv.ParentId = partition.StageId;
            pcv.UserId = partition.Manager.Id;
            
            //в post методе проверить изменился ли менеджер, и только если да - выполнить запрос на поиск нового. не создавать новый экземпляр partition, а использовать созданный!
            ViewBag.PartitionManagerUserName = partition.Manager.UserName;
            ViewData["UserId"] = ProjectController.GetSelectedListUsersByRoles(db, new List<string>() { "исполнитель", "руководитель направления" },partition.Manager.Id);
            FillViewBag(pcv.ParentId);
            return View(pcv);
        }

        // POST: /Partition/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Главный инженер проекта, Руководитель направления, Администратор")]
        public ActionResult Edit(PartitionCreateView pcv)//[Bind(Include="Id,StageId,CreationDate,DeadLine,StateId,Name,Description,IsPreFinish,IsFinish")] Partition partition)
        {
            Partition partition = db.Partitions.Find(pcv.Id);
            if(partition == null)
                return HttpNotFound();
            //if((partition.IsFinish ~ pcv.IsFinish) && (partition.IsPreFinish ~ pcv.IsPreFinish) && string.IsNullOrEmpty(pcv.Comment))
            if (string.IsNullOrEmpty((ModelState.Values.ElementAt(ModelState.Values.Count - 1).Value.RawValue as string[])[0]))
            {
                ModelState.Values.ElementAt(ModelState.Values.Count - 1).Errors.Add("Необходимо ввести комментарий!");
                FillViewBag(pcv.ParentId);
                return View(pcv);
            }
            if (ModelState.IsValid)
            {
                
                partition.Name = pcv.Name;
                partition.Description = pcv.Description;
                partition.DeadLine = pcv.DeadLine;    
                #region Состояние раздела
                if(partition.IsFinish ^ pcv.IsFinish)
                {
                    partition.IsFinish = pcv.IsFinish;
                    if(pcv.IsFinish)
                    {
                        partition.State = ProjectController.GetTaskState(db, "завершено");//db.TasksStates.Where(s=>s.Name.ToLower()=="завершено").First();
                        partition.StateId = partition.State.Id;
                        //создание события о завершении раздела
                    }
                    else
                    {
                        partition.IsPreFinish = false;
                        partition.State = ProjectController.GetTaskState(db, "доработка");//db.TasksStates.Where(s=>s.Name.Trim().ToLower()=="доработка").First();
                        partition.StateId = partition.State.Id;                        
                        //создание события об отправлении на доработку
                        //комментарий
                    }
                }
                else
                {
                    if (partition.IsPreFinish ^ pcv.IsPreFinish)
                    {
                        partition.IsPreFinish = pcv.IsPreFinish;
                        if (pcv.IsPreFinish)
                        {
                            partition.State = ProjectController.GetTaskState(db, "ожидает проверки");//db.TasksStates.Where(s => s.Name.Trim().ToLower() == "ожидает проверки").First();
                            partition.StateId = partition.State.Id;
                            //создание события о предварительной готовности, оповещение ГИП
                        }
                        else
                        {
                            partition.IsPreFinish = false;
                            partition.State = ProjectController.GetTaskState(db, "в работе");//db.TasksStates.Where(s => s.Name.Trim().ToLower() == "в работе").First();
                            partition.StateId = partition.State.Id;
                            //создание события об изменении статуса
                        }
                    }
                }
                
                if(!string.IsNullOrEmpty(pcv.UserId))
                {
                    if(partition.Manager == null || partition.Manager != null && partition.Manager.Id != pcv.UserId)
                    {
                        partition.Manager = db.Users.Where(u=>u.Id==pcv.UserId).Select(user=>user).First();
                        partition.State = ProjectStateController.GetState(db, "в работе");//db.TasksStates.Where(s=>s.Name.ToLower()=="в работе").First();
                        partition.StateId = partition.State.Id;
                    }
                }
                else
                    if(partition.Manager != null && string.IsNullOrEmpty(pcv.UserId))
                    {
                        partition.Manager = null;
                        partition.State = ProjectStateController.GetState(db, "отсутствует менеджер");//db.TasksStates.Where(s=>s.Name.ToLower()=="отсутствует менеджер").First();
                        partition.StateId = partition.State.Id;
                    }
                #endregion
                Comment comment = CommentController.AddComment(db, pcv.Comment, "разделы", partition.Id, User.Identity.Name, true);
                ApplicationUser auser = db.Users.Where(us => us.UserName == User.Identity.Name).Select(u => u).First();
                Event e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "изменил(а)"), auser, DependencyTable.Partition, partition);
                var ausers = GetUsersForPartitionEvent(partition);
                EventController.CreateEventForUsers(db, e, ausers);
                db.Entry(partition).State = EntityState.Modified;
                e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "добавил(а) коментарий"), auser, DependencyTable.Comment, comment);
                EventController.CreateEventForUsers(db, e, ausers);
                //событие оставлен комментарий или изменен раздел
                db.Entry(partition).State = EntityState.Modified;
                SaveDB(db);
                FillViewBag(pcv.ParentId);
                return RedirectToAction("Index", new { id = pcv.ParentId });
            }
            ViewData["UserId"] = ProjectController.GetSelectedListUsersByRoles(db, new List<string>() { "исполнитель", "руководитель направления" }, partition.Manager.Id);
            FillViewBag(pcv.ParentId);
            return View(pcv);
        }

        // GET: /Partition/Delete/5
        [Authorize(Roles = "Директор, Главный инженер проекта, Руководитель направления, Администратор")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Partition partition = db.Partitions.Find(id);
            if (partition == null)
            {
                return HttpNotFound();
            }
            FillViewBag(partition.StageId);
            return View(partition);
        }

        // POST: /Partition/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Главный инженер проекта, Руководитель направления, Администратор")]
        public ActionResult DeleteConfirmed(int id)
        {
            Partition partition = db.Partitions.Find(id);
            db.Partitions.Remove(partition);
            SaveDB(db);
            FillViewBag(partition.StageId);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
