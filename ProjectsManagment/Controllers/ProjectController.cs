using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectsManagment.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ProjectsManagment.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Project/
        public ActionResult Index()
        {
            var projects = db.Projects.Select(p => p);
            return View(projects.ToList());
        }

        public ActionResult GetFiles(int id)
        {
            return View();
        }

        public ActionResult Show(Project project)
        {
            return PartialView("ProjectInfo", project);
        }

        public ActionResult ShowDetails(Project project)
        {
            return PartialView("ProjectDetails", project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Главный инженер проекта, Администратор")]
        public ActionResult ShowInfoForCancel(int? id, bool details)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Project project = db.Projects.Find((int)id);
            if (project == null)
                return HttpNotFound();
            Comment comment = new Comment();
            comment.ItemId = (int)id;
            ViewBag.Error = "";
            ViewBag.Table = "Project";
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
            Project project = db.Projects.Find((int)id);
            if (project == null)
                return HttpNotFound();
            project.IsPreFinish = false;
            project.IsFinish = true;
            project.State = ProjectStateController.GetState(db, "завершено");//db.TasksStates.Where(ts => ts.Name.ToLower() == "завершено").Select(s => s).First();
            project.StateId = project.State.Id;
            project.Creator = db.Users.Find(project.Creator.Id);
            db.Entry(project).State = EntityState.Modified;
            ApplicationUser user = db.Users.Where(u => u.UserName == User.Identity.Name).Select(us => us).First();
            Event e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "отметил(а) как завершенный"), user, DependencyTable.Project, project);
            EventController.CreateEventForUsers(db, e, GetUsersForProjectEvent(project));
            SaveDB(db);
            //db.SaveChanges();
            if (!details)
                return Show(project);
            else
                return ShowDetails(project);
        }

        public Project CF(Comment comment)
        {
            Project project = db.Projects.Find(comment.ItemId);
            project.IsPreFinish = false;
            project.IsFinish = false;
            project.State = ProjectStateController.GetState(db, "доработка");//db.TasksStates.Where(ts => ts.Name.ToLower() == "доработка").Select(s => s).First();
            project.StateId = project.State.Id;
            project.Creator = db.Users.Find(project.Creator.Id);
            ApplicationUser user = db.Users.Where(us => us.UserName == User.Identity.Name).Select(u => u).First();
            Event e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "отправил(а) на доработку"), user, DependencyTable.Project, project);
            var users = GetUsersForProjectEvent(project);
            EventController.CreateEventForUsers(db, e, users);
            db.Entry(project).State = EntityState.Modified;
            comment = CommentController.AddComment(db, comment.Text, "проекты", comment.ItemId, user, true);
            e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "оставил(а) замечание"), user, DependencyTable.Comment, comment);
            EventController.CreateEventForUsers(db, e, users);
            SaveDB(db);
            return project;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Главный инженер проекта, Администратор")]
        public ActionResult CancelFinish(Comment comment)
        {
            if (string.IsNullOrEmpty(comment.Text))
            {
                ViewBag.Error = "Необходимо ввести замечание";
                ViewBag.Table = "Project";
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
                ViewBag.Table = "Project";
                return PartialView(comment);
            }
            else
            {
               
                return View("Details", CF(comment));
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


        // GET: /Project/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.TableId = TableTypesController.GetTableTypeId(db, "проекты");//db.TableTypes.Where(t => t.TableName.Trim().ToLower() == "проекты").Select(table => table.Id).First();
            return View(project);
        }

        // GET: /Project/Create
        [Authorize(Roles = "Директор, Администратор, Главный инженер проекта")]
        public ActionResult Create()
        {
            Project project = new Project();
            project.DeadLine = DateTime.Now.AddDays(1);
            ViewData["UserId"] = ProjectController.GetSelectedListUsersByRoles(db, new List<string>() { "директор", "главный инженер проекта" });
            return View();
        }

        // POST: /Project/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Администратор, Главный инженер проекта")]
        public ActionResult Create(/*[Bind(Include="Id,CreationDate,DeadLine,StateId,Name,Description,IsPreFinish,IsFinish,Creator")]*/ ProjectView pView)
        {
            
            if (ModelState.IsValid)
            {
                Project project = new Project();
                project.Name = pView.Name;
                project.Description = pView.Description;
                project.CreationDate = DateTime.Now;
                project.DeadLine = pView.DeadLine;
                if (!string.IsNullOrEmpty(pView.UserId))
                    project.Manager = db.Users.Where(u => u.Id == pView.UserId).Select(user => user).First();
                else
                    project.Manager = null;
                project.Creator = db.Users.Where(u => u.UserName == User.Identity.Name).Select(n => n).First();
                project.State = ProjectStateController.GetState(db, "в работе");//db.TasksStates.Where(st => st.Name.ToLower() == "в работе").Select(s => s).First();
                project.StateId = project.State.Id;
                db.Projects.Add(project);
                Event e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "создал(а)"), project.Creator, DependencyTable.Project, project);
                EventController.CreateEventForUsers(db, e, GetUsersForProjectEvent(project));
                SaveDB(db);
                return RedirectToAction("Index");
            }
            ViewData["UserId"] = ProjectController.GetSelectedListUsersByRoles(db, new List<string>() { "директор", "главный инженер проекта" });
            //ViewBag.StateId = new SelectList(db.TasksStates, "Id", "Name", project.StateId);
            return View(pView);
        }

        // GET: /Project/Edit/5
        [Authorize(Roles = "Директор, Администратор, Главный инженер проекта")]//Question
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            ProjectView pView = new ProjectView();
            pView.Id = project.Id;
            pView.Name = project.Name;
            pView.Description = project.Description;
            pView.DeadLine = project.DeadLine;
            if(project.Manager!=null)
                pView.UserId = project.Manager.Id;
            //ViewBag.StateId = new SelectList(db.TasksStates, "Id", "Name", project.StateId);
            ViewData["UserId"] = ProjectController.GetSelectedListUsersByRoles(db, new List<string>() { "главный инженер проекта", "директор" }, pView.UserId ?? string.Empty);
            return View(pView);
        }

        // POST: /Project/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Администратор, Главный инженер проекта")]
        public ActionResult Edit(ProjectView pView)
        {
            if (string.IsNullOrEmpty((ModelState.Values.ElementAt(ModelState.Values.Count - 1).Value.RawValue as string[])[0]))
            {
                ModelState.Values.ElementAt(ModelState.Values.Count - 1).Errors.Add("Необходимо ввести комментарий!");
                return View(pView);
            }
            if (ModelState.IsValid)
            {
                Project project = db.Projects.Find(pView.Id);
                if(project == null)
                    return HttpNotFound();
                project.Name = pView.Name;
                project.Description = pView.Description;
                project.DeadLine = pView.DeadLine;
                var temp = db.Users.Where(u=>u.Id == project.Creator.Id).Select(user=>user);
                if (temp == null)
                    project.Creator = db.Users.Where(u => u.UserName == User.Identity.Name).Select(user => user).First();
                if(project.IsFinish && !pView.IsFinish)
                {
                    project.State = ProjectStateController.GetState(db, "завершено");//db.TasksStates.Where(state => state.Name.ToLower() == "завершено").Select(s => s).First();
                    project.StateId = project.State.Id;
                }
                if (project.Manager != null && project.Manager.Id != pView.UserId || project.Manager == null)
                    project.Manager = db.Users.Where(u => u.Id == pView.UserId).Select(user => user).First();
                ApplicationUser auser = db.Users.Where(u => u.UserName == User.Identity.Name).Select(us => us).First();
                var users = GetUsersForProjectEvent(project);
                Comment comment = CommentController.AddComment(db, pView.Comment, "проекты", pView.Id, auser, false);
                Event e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "изменил(а)"), auser, DependencyTable.Project, project);
                EventController.CreateEventForUsers(db, e, users);
                e = EventController.CreateEvent(db, DateTime.Now, EventTypesController.GetEventType(db, "добавил(а) комментарий"), auser, DependencyTable.Comment, comment);
                EventController.CreateEventForUsers(db, e, users);
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewData["UserId"] = ProjectController.GetSelectedListUsersByRoles(db, new List<string>() { "директор", "главный инженер проекта" }, pView.UserId);
            return View(pView);
        }

        // GET: /Project/Delete/5
        [Authorize(Roles = "Директор, Администратор, Главный инженер проекта")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: /Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Директор, Администратор, Главный инженер проекта")]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            IEnumerable<Comment> comments;
            IEnumerable<Event> events;
            int tableId;
            if (project.Stages.Count > 0)
            {
                
                
                foreach (var stage in project.Stages.ToList())
                {
                    if (stage.Partitions.Count > 0)
                        foreach (var part in stage.Partitions.ToList())
                        {
                            if (part.Tasks.Count > 0)
                            {
                                IEnumerable<Dependency> deps;
                                foreach (var task in part.Tasks.ToList())
                                {
                                    tableId = TableTypesController.GetTableTypeId(db, "задания");
                                    events = db.Events.Where(e => e.EventTask.Id == task.Id || e.TableId == tableId && e.ItemId == task.Id).Select(ev => ev);
                                    db.Events.RemoveRange(events);
                                    comments = db.Comments.Where(c => c.TableId == tableId && c.ItemId == task.Id || c.Task.Id == task.Id).Select(comm => comm);
                                    db.Comments.RemoveRange(comments);
                                    deps = db.Dependencies.Where(d => d.TaskDep.Id == task.Id || d.Task.Id == task.Id).Select(dep => dep);
                                    db.Dependencies.RemoveRange(deps);
                                    
                                    db.Tasks.Remove(task);
                                }
                            }
                            
                            tableId = TableTypesController.GetTableTypeId(db, "разделы");
                            events = db.Events.Where(e => e.EventPartition.Id == part.Id || e.TableId == tableId && e.ItemId == part.Id).Select(ev => ev);
                            db.Events.RemoveRange(events);
                            comments = db.Comments.Where(c => c.TableId == tableId && c.ItemId == part.Id || c.Partition.Id == part.Id).Select(comm => comm);
                            db.Comments.RemoveRange(comments);                           
                            
                            db.Partitions.Remove(part);
                        }
                    tableId = TableTypesController.GetTableTypeId(db, "стадии");
                    events = db.Events.Where(e => e.EventStage.Id == stage.Id || e.TableId == tableId && e.ItemId == stage.Id).Select(ev => ev);
                    db.Events.RemoveRange(events);
                    comments = db.Comments.Where(c => c.TableId == tableId && c.ItemId == stage.Id || c.Stage.Id == stage.Id).Select(comm => comm);
                    db.Comments.RemoveRange(comments);                           
                   
                    db.Stages.Remove(stage);
                }
            }
            tableId = TableTypesController.GetTableTypeId(db, "проекты");
            events = db.Events.Where(e => e.EventProject.Id == project.Id || e.TableId == tableId && e.ItemId == project.Id).Select(ev => ev);
            db.Events.RemoveRange(events);
            comments = db.Comments.Where(c => c.TableId == tableId && c.ItemId == project.Id || c.Project.Id == project.Id).Select(comm => comm);
            db.Comments.RemoveRange(comments);                           
           
            db.Projects.Remove(project);
            SaveDB(db);
            return RedirectToAction("Index");
        }

        public ActionResult GetComments(int projectId, int count)
        {
            int tableId = TableTypesController.GetTableTypeId(db, "проекты");//db.TableTypes.Where(t => t.TableName.Trim().ToLower() == "проекты").Select(table => table.Id).First();
            return PartialView("CommentSummary", CommentController.GetCommentsFor(db, tableId, projectId, count));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        private IEnumerable<ApplicationUser> GetUsersForProjectEvent(Project project)
        {
            List<ApplicationUser> result = new List<ApplicationUser>();
            if (project.Manager != null)
                result.Add(project.Manager);
            var directors = GetUsersByRoles(db, new List<string>() { "директор" });
            foreach (var user in directors)
                if (result.Find(u => u.Id == user.Id) == null)
                    result.Add(user);
            return result;
        }

        public static IEnumerable<ApplicationUser> GetUsersByRoles(ApplicationDbContext context, List<string> roles)
        {
            try
            {
                var rolesId = context.Roles.Where(role => roles.Contains(role.Name.ToLower())).Select(r => r.Id);
                IEnumerable<ApplicationUser> users = context.Users.Where(user => user.Roles.Select(r => r.RoleId).Intersect(rolesId).Count() > 0).Select(u => u);
                return users;
            }
            catch
            {
                return null;
            }
        }

        public static IEnumerable<SelectListItem> GetSelectedListUsersByRoles(ApplicationDbContext context, List<string> roles)
        {

            IEnumerable<ApplicationUser> users = GetUsersByRoles(context, roles);
            IEnumerable<SelectListItem> selectList =
                from c in users
                select new SelectListItem
                {
                    Text = c.Surname + " " + c.Name + " " + c.Fathername,
                    Value = c.Id
                };
            return selectList;
        }

        public static IEnumerable<SelectListItem> GetSelectedListUsersByRoles(ApplicationDbContext context, List<string> roles, string selectedValue)
        {
            IEnumerable<ApplicationUser> users = GetUsersByRoles(context, roles);
            IEnumerable<SelectListItem> selectList =
                from c in users
                select new SelectListItem
                {
                    Text = c.Surname + " " + c.Name + " " + c.Fathername,
                    Value = c.Id,
                    Selected = c.Id == selectedValue
                };
            
            return selectList;
        }

        public static TasksState GetTaskState(ApplicationDbContext context, string taskStateName)
        {
            var tts = context.TasksStates.Where(t => t.Name.ToLower() == taskStateName.ToLower()).Select(tt => tt);
            if (tts == null || tts.Count() == 0)
            {
                TasksState tt = new TasksState();
                tt.Name = taskStateName;
                context.TasksStates.Add(tt);
                SaveDB(context);
                return tt;
            }
            else
                return tts.First();
        }

        
    }
}
