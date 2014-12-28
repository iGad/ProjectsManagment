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
    public class StageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Stage/
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Id = id;
            ViewBag.ProjectName = db.Projects.Where(s => s.Id == id).Select(p => p.Name).First();
            var stages = db.Stages.Where(a => a.ProjectId == id).Select(s => s);//.Include(s => s.Project.Id == id);
            FillViewBag((int)id);
            if (stages != null)
            {
                //var res = stages.Include(l => l);
                return View(stages.ToList());
            }
            else return View((IEnumerable<Stage>)null);
            
        }

        public ActionResult Show(Stage stage)
        {
            return PartialView("StageInfo", stage);
        }

        public ActionResult ShowDetails(Stage stage)
        {
            return PartialView("StageDetails", stage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Главный инженер проекта, Администратор")]
        public ActionResult ShowInfoForCancel(int? id, bool details)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Stage stage = db.Stages.Find((int)id);
            if (stage == null)
                return HttpNotFound();
            Comment comment = new Comment();
            comment.ItemId = (int)id;
            ViewBag.Error = "";
            ViewBag.Table = "Stage";
            ViewBag.Details = details;
            return PartialView("AddCancelComment", comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Главный инженер проекта, Администратор")]
        public ActionResult MakeFinish(int? id, bool details)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Stage stage = db.Stages.Find((int)id);
            if (stage == null)
                return HttpNotFound();
            stage.IsPreFinish = false;
            stage.IsFinish = true;
            stage.State = ProjectStateController.GetState(db, "завершено");//db.TasksStates.Where(ts => ts.Name.ToLower() == "завершено").Select(s => s).First();
            stage.StateId = stage.State.Id;
            stage.Creator = db.Users.Find(stage.Creator.Id);
            db.Entry(stage).State = EntityState.Modified;
            ApplicationUser user = db.Users.Where(u => u.UserName == User.Identity.Name).Select(us => us).First();
            Event e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "отметил(а) как завершенный"), user, DependencyTable.Stage, stage);
            EventController.CreateEventForUsers(db, e, GetUsersForStageEvent(stage));
            SaveDB(db);
            //db.SaveChanges();
            if (!details)
                return Show(stage);
            else
                return ShowDetails(stage);
        }

        private IEnumerable<ApplicationUser> GetUsersForStageEvent(Stage stage)
        {
            List<ApplicationUser> result = new List<ApplicationUser>();
            if (stage.Executor != null)
                result.Add(stage.Executor);
            var directors = ProjectController.GetUsersByRoles(db, new List<string>() { "директор", "главный инженер проекта" });
            foreach (var user in directors)
                if (result.Find(u => u.Id == user.Id) == null)
                    result.Add(user);
            return result;
        }

        public Stage CF(Comment comment)
        {
            Stage stage = db.Stages.Find(comment.ItemId);
            stage.IsPreFinish = false;
            stage.IsFinish = false;
            stage.State = ProjectStateController.GetState(db, "доработка");//db.TasksStates.Where(ts => ts.Name.ToLower() == "доработка").Select(s => s).First();
            stage.StateId = stage.State.Id;
            stage.Creator = db.Users.Find(stage.Creator.Id);
            ApplicationUser user = db.Users.Where(us => us.UserName == User.Identity.Name).Select(u => u).First();
            Event e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "отправил(а) на доработку"), user, DependencyTable.Stage, stage);
            var users = GetUsersForStageEvent(stage);
            EventController.CreateEventForUsers(db, e, users);
            db.Entry(stage).State = EntityState.Modified;
            comment = CommentController.AddComment(db, comment.Text, "стадии", comment.ItemId, user, true);
            e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "оставил(а) замечание"), user, DependencyTable.Comment, comment);
            EventController.CreateEventForUsers(db, e, users);
            SaveDB(db);
            return stage;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Главный инженер проекта, Администратор")]
        public ActionResult CancelFinish(Comment comment)
        {
            if (string.IsNullOrEmpty(comment.Text))
            {
                ViewBag.Error = "Необходимо ввести замечание";
                ViewBag.Table = "Stage";
                return PartialView(comment);
            }
            else
            {
               
                return Show(CF(comment));
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Главный инженер проекта, Администратор")]
        public ActionResult CancelFinish2(Comment comment)
        {
            if (string.IsNullOrEmpty(comment.Text))
            {
                ViewBag.Error = "Необходимо ввести замечание";
                ViewBag.Table = "Stage";
                return PartialView(comment);
            }
            else
            {               
                return View("Details",CF(comment));
            }

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

        private void FillViewBag(int id)
        {
            Project part = db.Projects.Find(id);
            ViewBag.ProjectName = part.Name;
            ViewBag.ProjectId = id;
        }

        public ActionResult GetComments(int stageId, int count)
        {
            int tableId = TableTypesController.GetTableTypeId(db, "стадии"); //db.TableTypes.Where(t => t.TableName.Trim().ToLower() == "стадии").Select(table => table.Id).First();
            return PartialView("CommentSummary", CommentController.GetCommentsFor(db, tableId, stageId, count));
        }

        // GET: /Stage/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stage stage = db.Stages.Find(id);
            if (stage == null)
            {
                return HttpNotFound();
            }
            ViewBag.TableId = TableTypesController.GetTableTypeId(db, "стадии"); //db.TableTypes.Where(t => t.TableName.Trim().ToLower() == "стадии").Select(table => table.Id).First();
            FillViewBag(stage.ProjectId);
            return View(stage);
        }
         [Authorize(Roles = "Администратор, Директор,Главный инженер проекта")]
        // GET: /Stage/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StageView cppv = new StageView();
            cppv.ParentId = (int)id;
            ViewData["UserId"] = ProjectController.GetSelectedListUsersByRoles(db, new List<string>() { "директор", "главный инженер проекта" });
            FillViewBag((int)id);
            return View(cppv);
        }

        // POST: /Stage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Администратор, Директор,Главный инженер проекта")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StageView cppv)
        {
            
            if (ModelState.IsValid)
            {
                Stage stage = new Stage();
                stage.Creator = db.Users.Where(u => u.UserName == User.Identity.Name).Select(n => n).First();
                if (stage.Creator == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                if (!string.IsNullOrEmpty(cppv.UserId))
                    stage.Executor = db.Users.Where(u => u.Id == cppv.UserId).Select(user => user).First();
                else
                    stage.Executor = null;
                stage.Name = cppv.Name;
                stage.Description = cppv.Description;
                stage.CreationDate = DateTime.Now;
                stage.IsFinish = stage.IsPreFinish = false;
                stage.ProjectId = cppv.ParentId;
                stage.Project = db.Projects.Where(p => p.Id == stage.ProjectId).Select(pr => pr).First();
                stage.State = ProjectStateController.GetState(db, "в работе");// db.TasksStates.Where(s => s.Name.ToLower() == "в работе").Select(t => t.Id).First();
                stage.StateId = stage.State.Id;
                db.Stages.Add(stage);
                Event e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "создал(а)"), stage.Creator, DependencyTable.Stage, stage);
                EventController.CreateEventForUsers(db, e, GetUsersForStageEvent(stage));
                SaveDB(db);
                FillViewBag(stage.ProjectId);
                return RedirectToAction("Index", new { id = stage.ProjectId });
            }
            
            return View(cppv);
        }

        // GET: /Stage/Edit/5
         [Authorize(Roles = "Администратор, Директор, Главный инженер проекта")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stage stage = db.Stages.Find(id);
            if (stage == null)
            {
                return HttpNotFound();
            }
            StageView stageView = new StageView();
            stageView.Name = stage.Name;
            stageView.Description = stage.Description;
            stageView.IsFinish = stage.IsFinish;
            stageView.Id = stage.Id;
            stageView.ParentId = stage.ProjectId;
            ViewData["UserId"] = ProjectController.GetSelectedListUsersByRoles(db, new List<string>() { "главный инженер проекта", "директор" }, stageView.UserId ?? string.Empty);
            FillViewBag(stage.ProjectId);
            return View(stageView);
        }

        // POST: /Stage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор, Директор, Главный инженер проекта")]
        public ActionResult Edit(StageView stageView)
        {
            if (string.IsNullOrEmpty((ModelState.Values.ElementAt(ModelState.Values.Count - 1).Value.RawValue as string[])[0]))
            {
                ModelState.Values.ElementAt(ModelState.Values.Count - 1).Errors.Add("Необходимо ввести комментарий!");
                ViewData["UserId"] = ProjectController.GetSelectedListUsersByRoles(db, new List<string>() { "главный инженер проекта", "директор" }, stageView.UserId ?? string.Empty);
                FillViewBag(stageView.ParentId);
                return View(stageView);
            }
            if (ModelState.IsValid)
            {
                Stage stage = db.Stages.Find(stageView.Id);
                if (stage == null)
                    return HttpNotFound();
                stage.Name = stageView.Name;
                stage.Description = stageView.Description;
                if(stage.IsFinish && !stageView.IsFinish)
                {
                    stage.State = ProjectController.GetTaskState(db, "доработка");//db.TasksStates.Where(s => s.Name.ToLower() == "доработка").Select(t => t).First();
                    stage.StateId = stage.State.Id;
                }
                if(!stage.IsFinish && stageView.IsFinish)
                {
                    stage.State = ProjectController.GetTaskState(db, "завершено");//db.TasksStates.Where(s => s.Name.ToLower() == "завершено").Select(t => t).First();
                    stage.StateId = stage.State.Id;
                }
                db.Entry(stage).State = EntityState.Modified;
                ApplicationUser user = db.Users.Where(us => us.UserName == User.Identity.Name).Select(u => u).First();
                Comment comment = CommentController.AddComment(db, stageView.Comment, "стадии", stage.Id, user, true);
                Event e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "изменил(а)"), user, DependencyTable.Stage, stage);
                var users = GetUsersForStageEvent(stage);
                EventController.CreateEventForUsers(db, e, users);
                db.Entry(stage).State = EntityState.Modified;                
                e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "добавил(а) коментарий"), user, DependencyTable.Comment, comment);
                EventController.CreateEventForUsers(db, e, users);
                SaveDB(db);
                FillViewBag(stageView.ParentId);
                return RedirectToAction("Index", new { id = stage.ProjectId });
            }
            ViewData["UserId"] = ProjectController.GetSelectedListUsersByRoles(db, new List<string>() { "главный инженер проекта", "директор" }, stageView.UserId ?? string.Empty);
            FillViewBag(stageView.ParentId);
            return View(stageView);
        }

        // GET: /Stage/Delete/5
         [Authorize(Roles = "Администратор, Директор, Главный инженер проекта")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stage stage = db.Stages.Find(id);
            if (stage == null)
            {
                return HttpNotFound();
            }
            FillViewBag(stage.ProjectId);
            return View(stage);
        }

        // POST: /Stage/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Администратор, Директор, Главный инженер проекта")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Stage stage = db.Stages.Find(id);
            db.Stages.Remove(stage);
            SaveDB(db);
            FillViewBag(stage.ProjectId);
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
