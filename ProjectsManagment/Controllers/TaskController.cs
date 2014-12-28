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
    public class TaskController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
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
        // GET: /Task/
        public ActionResult Index(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ViewBag.Id = id;
            var partition = db.Partitions.Where(s => s.Id == id).Select(p => p).First();
            if (partition != null)
                ViewBag.PartitionName = partition.Name;//db.Stages.Where(s => s.Id == id).Select(p => p.Name).First();
            else
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ViewBag.StageName = partition.Stage.Name;
            ViewBag.ProjectName = partition.Stage.Project.Name;

            var tasks = db.Tasks.Where(p => p.PartitionId == id).Select(s => s);
            FillViewBag(partition.Id);
            ViewBag.My = false;
            return View(tasks.ToList());
        }

        public ActionResult MyTasks()
        {
            ApplicationUser iam = db.Users.Where(u => u.UserName == User.Identity.Name).Select(user => user).First();
            var tasks = db.Tasks.Where(t => t.Executor.Id == iam.Id).Select(task => task);
            ViewBag.My = true;
            return View("Index", tasks);
        }

        private IEnumerable<ApplicationUser> GetUsersForTaskEvent(Task task)
        {
            List<ApplicationUser> result = new List<ApplicationUser>();
            if (task.Executor != null)
                result.Add(task.Executor);
            if (task.Partition.Manager != null && result.Find(user=>user.Id == task.Partition.Manager.Id)==null)
                result.Add(task.Partition.Manager);
            if (task.Partition.Stage.Executor != null && result.Find(user => user.Id == task.Partition.Stage.Executor.Id) == null)
                result.Add(task.Partition.Stage.Executor);
            if (task.Partition.Stage.Project.Manager != null && result.Find(user => user.Id == task.Partition.Stage.Project.Manager.Id) == null)
                result.Add(task.Partition.Stage.Project.Manager);

            var directors = ProjectController.GetUsersByRoles(db, new List<string>() { "директор" });//db.Users.Where(user => user.Roles.Where(ur => ur.RoleId == directorRoleId).Select(urs => urs).Count() > 0).Select(u => u);
            foreach (var u in directors)
                if (result.Find(user => user.Id == u.Id) == null)
                    result.Add(u);
            return result;
        }

        public ActionResult Show(Task task)
        {
            if (task == null)
                return HttpNotFound();
            else
                return PartialView("TaskInfo", task);
        }

        public ActionResult ShowDetails(Task task)
        {
            if (task == null)
                return HttpNotFound();
            else
            return PartialView("TaskDetails", task);
        }

        public ActionResult ShowById(int id)
        {
            Task task = db.Tasks.Find(id);
            if (task == null)
                return HttpNotFound();
            ViewBag.Comment = false;
            
            return PartialView("TaskInfo", task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Главный инженер проекта, Руководитель направления, Администратор")]
        public ActionResult ShowInfoForCancel(int? id, bool details)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Task task = db.Tasks.Find((int)id);
            if (task == null)
                return HttpNotFound();
            Comment comment = new Comment();
            comment.ItemId = (int)id;
            ViewBag.Error = "";
            ViewBag.Table = "Task";
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
            Task task = db.Tasks.Find((int)id);
            if (task == null)
                return HttpNotFound();
            ApplicationUser user = db.Users.Where(us => us.UserName == User.Identity.Name).Select(u => u).First();
            Event e;
            if (task.Executor != null && user.UserName == task.Executor.UserName)
            {
                task.IsPreFinish = true;
                task.State = ProjectStateController.GetState(db, "ожидает проверки"); //db.TasksStates.Where(ts => ts.Name.ToLower() == "ожидает проверки").Select(s => s).First();
                task.StateId = task.State.Id;
                e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "отметил(а) как завершенное"), user, DependencyTable.Task, task);
            }
            else
            {                
                task.IsPreFinish = false;
                task.IsFinish = true;
                task.State = ProjectStateController.GetState(db, "завершено");//db.TasksStates.Where(ts => ts.Name.ToLower() == "завершено").Select(s => s).First();
                task.StateId = task.State.Id;
                e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "подтвердил(а) завершение"), user, DependencyTable.Task, task);
            }
            
            task.Creator = db.Users.Find(task.Creator.Id);
            db.Entry(task).State = EntityState.Modified;
            EventController.CreateEventForUsers(db, e, GetUsersForTaskEvent(task));
            SaveDB(db);
            //db.SaveChanges();
            if (details)
                return ShowDetails(task);
            else
                return Show(task);
        }

        public Task CF(Comment comment)
        {
            Task task = db.Tasks.Find(comment.ItemId);
            task.IsPreFinish = false;
            task.IsFinish = false;
            task.State = ProjectStateController.GetState(db, "доработка");//db.TasksStates.Where(ts => ts.Name.ToLower() == "доработка").Select(s => s).First();
            task.StateId = task.State.Id;
            task.Creator = db.Users.Find(task.Creator.Id);            
            ApplicationUser user = db.Users.Where(us => us.UserName == User.Identity.Name).Select(u => u).First();
            var users = GetUsersForTaskEvent(task);
            Event e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "отправил(а) на доработку"), user, DependencyTable.Task, task);
            EventController.CreateEventForUsers(db, e, users);
            db.Entry(task).State = EntityState.Modified;
            comment = CommentController.AddComment(db, comment.Text, "задания", comment.ItemId, user,true);
            e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "оставил(а) замечание"), user, DependencyTable.Comment, comment);
            SaveDB(db);
            return task;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Главный инженер проекта, Руководитель направления, Администратор")]
        public ActionResult CancelFinish(Comment comment)
        {
            if (string.IsNullOrEmpty(comment.Text))
            {
                ViewBag.Error = "Необходимо ввести замечание";
                ViewBag.Table = "Task";
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
                ViewBag.Table = "Task";
                return PartialView(comment);
            }
            else
            {
               
                return View("Details",CF(comment));
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakePreFinish(int? id, bool details)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Task task = db.Tasks.Find((int)id);
            if (task == null)
                return HttpNotFound();
            task.IsPreFinish = true;
            task.State = ProjectStateController.GetState(db, "ожидает проверки");//db.TasksStates.Where(ts => ts.Name.ToLower() == "ожидает проверки").Select(s => s).First();
            task.StateId = task.State.Id;
            task.Creator = db.Users.Find(task.Creator.Id);
            db.Entry(task).State = EntityState.Modified;
            SaveDB(db);
            if (details)
                return ShowDetails(task);
            else
                return Show(task);
        }

        private void FillViewBag(int id)
        {
            Partition part = db.Partitions.Find(id);
            ViewBag.StageName = part.Stage.Name;
            ViewBag.StageId = part.StageId;
            ViewBag.ProjectName = part.Stage.Project.Name;
            ViewBag.ProjectId = part.Stage.ProjectId;
            ViewBag.PartitionName = part.Name;
            ViewBag.PartitionId = id;
        }
        
        public ActionResult GetComments(int taskId, int count)
        {
            int tableId = TableTypesController.GetTableTypeId(db, "задания");//db.TableTypes.Where(t => t.TableName.Trim().ToLower() == "задания").Select(table => table.Id).First();
            return PartialView("CommentSummary", CommentController.GetCommentsFor(db, tableId, taskId, count));
        }

        // GET: /Task/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            ViewBag.TableId = TableTypesController.GetTableTypeId(db, "задания");//db.TableTypes.Where(t => t.TableName.Trim().ToLower() == "задания").Select(table => table.Id).First();
            FillViewBag(task.PartitionId);
            return View(task);
        }

        // GET: /Task/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            TaskView tView = new TaskView();
            tView.DeadLine = DateTime.Now.AddDays(1);
            ViewBag.pId = id;
            ViewData["UserId"] = ProjectController.GetSelectedListUsersByRoles(db, new List<string>() { "исполнитель" });//Question  
            FillViewBag((int)id);
            return View(tView);
        }

        // POST: /Task/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Директор, Главный инженер проекта, Руководитель направления, Администратор")]
        public ActionResult Create(TaskView tView)
        {
            if (tView.ParentId <= 0 || db.Partitions.Where(stage => stage.Id == tView.ParentId).Select(s => s).Count() == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (ModelState.IsValid)
            {
                Task task = new Task();
                task.CreationDate = DateTime.Now;
                task.DeadLine = tView.DeadLine;
                task.Name = tView.Name;
                task.Description = tView.Description;
                if (string.IsNullOrEmpty(tView.UserId))
                {
                    task.State = ProjectController.GetTaskState(db,"отсутствует исполнитель");
                    task.Executor = null;
                }
                else
                {                    
                    task.State = ProjectController.GetTaskState(db, "в работе");                   
                    task.Executor = db.Users.Where(u => u.Id == tView.UserId).Select(user => user).First();
                }
                task.StateId = task.State.Id;
                task.Creator = db.Users.Where(u => u.UserName == User.Identity.Name).Select(n => n).First();
                task.Partition = db.Partitions.Where(p => p.Id == tView.ParentId).Select(s => s).First();
                task.PartitionId = task.Partition.Id;                
                db.Tasks.Add(task);
                Event e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "создал(а)"), task.Creator, DependencyTable.Task, task);
                EventController.CreateEventForUsers(db, e, GetUsersForTaskEvent(task));
                ProjectController.SaveDB(db);
                FillViewBag(tView.ParentId);
                return RedirectToAction("Index", new { id = tView.ParentId });
            }
            ViewBag.pId = tView.ParentId;
            ViewBag.DeadLine = tView.DeadLine;
            ViewData["UserId"] = ProjectController.GetSelectedListUsersByRoles(db, new List<string>() { "исполнитель" }, tView.UserId);
            FillViewBag(tView.ParentId);
            return View(tView);
        }

        [Authorize(Roles = "Директор, Администратор, Главный инженер проекта, Руководитель направления")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            TaskView tView = new TaskView();
            tView.DeadLine = task.DeadLine;
            tView.Description = task.Description;
            tView.Name = task.Name;
            tView.Id = task.Id;
            tView.IsFinish = task.IsFinish;
            tView.IsPreFinish = task.IsPreFinish;
            tView.ParentId = task.PartitionId;
            tView.UserId = task.Executor.Id;
            
            ViewBag.TaskExecutorUserName = task.Executor.UserName;
            ViewData["UserId"] = ProjectController.GetSelectedListUsersByRoles(db, new List<string>() { "исполнитель", "руководитель направления" }, task.Executor.Id);
            //selectList.Where(i => i.Value == tView.UserId).Select(item => item).First().Selected = true;
            FillViewBag(tView.ParentId);
            return View(tView);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Администратор, Главный инженер проекта, Руководитель направления")]
        public ActionResult Edit(TaskView tView)
        {
            Task task = db.Tasks.Find(tView.Id);
            if (task == null)
                return HttpNotFound();
            //if((task.IsFinish ~ tView.IsFinish) && (task.IsPreFinish ~ tView.IsPreFinish) && string.IsNullOrEmpty(tView.Comment))
            if (string.IsNullOrEmpty((ModelState.Values.ElementAt(ModelState.Values.Count - 1).Value.RawValue as string[])[0]))
            {
                ModelState.Values.ElementAt(ModelState.Values.Count - 1).Errors.Add("Необходимо ввести комментарий!");
                ViewData["UserId"] = ProjectController.GetSelectedListUsersByRoles(db, new List<string>() { "исполнитель" }, task.Executor.Id);
                return View(tView);
            }
            if (ModelState.IsValid)
            {

                task.Name = tView.Name;
                task.Description = tView.Description;
                task.DeadLine = tView.DeadLine;
                #region Состояние раздела
                if (task.IsFinish ^ tView.IsFinish)
                {
                    task.IsFinish = tView.IsFinish;
                    if (tView.IsFinish)
                    {
                       
                        task.State = ProjectStateController.GetState(db, "завершено"); //db.TasksStates.Where(s => s.Name.ToLower() == "завершено").First();
                        task.StateId = task.State.Id;
                        //создание события о завершении раздела
                    }
                    else
                    {
                        task.IsPreFinish = false;
                        task.State = ProjectStateController.GetState(db, "доработка");//db.TasksStates.Where(s => s.Name.Trim().ToLower() == "доработка").First();
                        task.StateId = task.State.Id;
                        //создание события об отправлении на доработку
                        //комментарий
                    }
                }
                else
                {
                    if (task.IsPreFinish ^ tView.IsPreFinish)
                    {
                        task.IsPreFinish = tView.IsPreFinish;
                        if (tView.IsPreFinish)
                        {
                            task.State = ProjectStateController.GetState(db, "ожидает проверки"); //db.TasksStates.Where(s => s.Name.Trim().ToLower() == "ожидает проверки").First();
                            
                            task.StateId = task.State.Id;
                            //создание события о предварительной готовности, оповещение ГИП
                        }
                        else
                        {
                            task.IsPreFinish = false;
                            task.State = ProjectStateController.GetState(db, "в работе"); //db.TasksStates.Where(s => s.Name.Trim().ToLower() == "в работе").First();
                            
                            task.StateId = task.State.Id;
                            //создание события об изменении статуса
                        }
                    }
                }

                if (!string.IsNullOrEmpty(tView.UserId))
                {
                    if (task.Executor == null || task.Executor != null && task.Executor.Id != tView.UserId)
                    {
                        task.Executor = db.Users.Where(u => u.Id == tView.UserId).Select(user => user).First();
                        task.State = ProjectStateController.GetState(db, "в работе"); //db.TasksStates.Where(s => s.Name.Trim().ToLower() == "в работе").First();
                        
                        task.StateId = task.State.Id;
                    }
                }
                else
                    if (task.Executor != null && string.IsNullOrEmpty(tView.UserId))
                    {
                        task.Executor = null;
                        task.State = ProjectStateController.GetState(db, "отсутствует исполнитель"); //db.TasksStates.Where(s => s.Name.ToLower() == "отсутствует исполнитель").First();
                                           
                        task.StateId = task.State.Id;
                    }
                #endregion
                
                //событие оставлен комментарий или изменен раздел
                task.Creator = db.Users.Find(task.Creator.Id);
                var users = GetUsersForTaskEvent(task);
                ApplicationUser auser = db.Users.Where(u => u.UserName == User.Identity.Name).Select(us => us).First();
                Comment comment = CommentController.AddComment(db, tView.Comment, "задания", tView.Id, auser, false);
                Event e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "создал(а)"), auser, DependencyTable.Task, task);
                EventController.CreateEventForUsers(db, e, users);
                e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "добавил(а) комментарий"), auser, DependencyTable.Comment, comment);
                EventController.CreateEventForUsers(db, e, users);
                db.Entry(task).State = EntityState.Modified;
                ProjectController.SaveDB(db);
                FillViewBag(tView.ParentId);
                return RedirectToAction("Index", new { id = tView.ParentId});
            }
            ViewData["UserId"] = ProjectController.GetSelectedListUsersByRoles(db, new List<string>() { "исполнитель" }, task.Executor.Id);
            FillViewBag(tView.ParentId);
            return View(tView);
        }

        public string GetUserNameById(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            return user.Surname + " " + user.Name + " " + user.Fathername;
        }

        // GET: /Task/Delete/5
        [Authorize(Roles = "Директор, Главный инженер проекта, Руководитель направления, Администратор")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            FillViewBag(task.PartitionId);
            ViewBag.DependTasks = db.Dependencies.Where(d => d.TaskDep.Id == task.Id).Select(dep => dep);
            return View(task);
        }

        // POST: /Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Главный инженер проекта, Руководитель направления, Администратор")]
        public ActionResult DeleteConfirmed(int id)
        {
            Task task = db.Tasks.Find(id);
            int partId = task.PartitionId;
            ApplicationUser user = db.Users.Where(u => u.UserName == User.Identity.Name).Select(us => us).First();
            Event e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "удалил(а)"), user, DependencyTable.Task, task);
            EventController.CreateEventForUsers(db, e, GetUsersForTaskEvent(task));
            db.Tasks.Remove(task);
            ProjectController.SaveDB(db);
            FillViewBag(partId);
            return RedirectToAction("Index", new { id = partId });
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
