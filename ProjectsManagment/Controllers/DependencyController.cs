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
    public class DependencyController : Controller
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


        // GET: /Dependency/
        public ActionResult Index(int? taskId)
        {
            if (taskId == null)
                return HttpNotFound();
            //var dependencies = db.Dependencies.Where(d => d.TableId == (int)tableId && d.ItemId == (int)itemId).Select(dep => dep);
            Task task = db.Tasks.Find(taskId);
            return PartialView(task);
        }

        public ActionResult ShowIncome(int taskId)
        {
            return PartialView("SummaryIncome", GetIncomeDependencies(db, 0, taskId));
        }

        public ActionResult ShowOutcome(int taskId)
        {
            return PartialView("SummaryOutcome", GetOutcomeDependencies(db, 0, taskId));
        }

        public ActionResult ShowInDependency(Dependency dependency)
        {
            return PartialView("IncomeDependency", dependency);
        }

        public ActionResult ShowOutDependency(Dependency dependency)
        {
            return PartialView("OutcomeDependency", dependency);
        }

        public ActionResult ShowAdd(int taskId)
        {
            Task task = db.Tasks.Find(taskId);
            ViewData["StageId"] = GetStagesByProjectId(task.Partition.Stage.ProjectId);
            ViewData["PartitionId"] = new List<SelectListItem>();
            ViewData["TaskId"] = new List<SelectListItem>();
            DependencyView dView = new DependencyView();
            dView.ItemId = taskId;
            return PartialView("Create", dView);
        }

        

        public IEnumerable<SelectListItem> GetStagesByProjectId(int projectId)
        {            
            var stages = db.Stages.Where(st=>st.ProjectId == projectId).Select(s => s);
            IEnumerable<SelectListItem> selectList =
                from c in stages
                select new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            return selectList;
        }
        public IEnumerable<SelectListItem> GetStagesByProjectId(int projectId, int selectedId)
        {
            var stages = db.Stages.Where(st => st.ProjectId == projectId).Select(s => s);
            IEnumerable<SelectListItem> selectList =
                from c in stages
                select new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                    Selected = c.Id == selectedId
                };
            return selectList;
        }

        public ActionResult GetPartitionsByStageId(string stageId)
        {
            if (String.IsNullOrEmpty(stageId))
            {
                throw new ArgumentNullException("stageId");
            }
            int id = 0;
            int sId;
            if (int.TryParse(stageId, out sId))
            {
                var entities = db.Partitions.Where(st => st.StageId == sId).Select(s => s);
                var result = (from s in entities
                              select new
                              {
                                  id = s.Id,
                                  name = s.Name
                              }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
                return HttpNotFound();
        }

        public ActionResult GetTasksByPartitionId(string partitionId)
        {
            if (String.IsNullOrEmpty(partitionId))
            {
                throw new ArgumentNullException("partitionId");
            }
            int id = 0;
            int pId;
            if (int.TryParse(partitionId, out pId))
            {
                var entities = db.Tasks.Where(st => st.PartitionId == pId).Select(s => s);
                var result = (from s in entities
                              select new
                              {
                                  id = s.Id,
                                  name = s.Name
                              }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else return HttpNotFound();
        } 

        public static IEnumerable<Dependency> GetIncomeDependencies(ApplicationDbContext context, int tableId, int itemId)
        {
            //TODO: узнать про не только задания
            //Task task = context.Tasks.Find(itemId);
            var dependencies = context.Dependencies.Where(d => d.ItemId == itemId).Select(dep => dep);
            return dependencies.ToList();
        }

        public static IEnumerable<Dependency> GetOutcomeDependencies(ApplicationDbContext context, int tableId, int itemId)
        {
            //TODO: узнать про не только задания
            //Task task = context.Tasks.Find(itemId);
            var dependencies = context.Dependencies.Where(d => d.TaskDep.Id == itemId).Select(dep => dep);
            return dependencies.ToList();
        }

        // POST: /Dependency/Create        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DependencyView dView)
        {            
            if (ModelState.IsValid)
            {
                if(db.Dependencies.Where(d => d.ItemId == dView.ItemId && d.TaskDep.Id == dView.TaskId).Select(c=>c).Count() == 0)
                {
                    Dependency dependency = new Dependency();
                    dependency.ItemId = dView.ItemId;
                    dependency.TableId = TableTypesController.GetTableTypeId(db, "задания");
                    dependency.Task = db.Tasks.Find(dView.ItemId);
                    dependency.TaskDep = db.Tasks.Find(dView.TaskId);
                    
                    db.Dependencies.Add(dependency);
                    SaveDB(db);
                    return ShowIncome(dView.ItemId);
                }
                ModelState.AddModelError("TaskId", "Такая зависимость уже существует");
            }
            //if (dView.TaskId <= 0 || dView.StageId <= 0 || dView.PartitionId <= 0)
            //    ModelState.AddModelError("TaskId", "Нельзя добавить такую зависимость");
            //Task task = db.Tasks.Find(dView.ItemId);
            //ViewData["StageId"] = GetStagesByProjectId(task.Partition.Stage.ProjectId, dView.StageId);
            //ViewData["PartitionId"] = new List<SelectListItem>();
            //ViewData["TaskId"] = new List<SelectListItem>();
            //return PartialView("Create", dView);
            return ShowIncome(dView.ItemId);
        }

       
        // GET: /Dependency/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dependency dependency = db.Dependencies.Find(id);
            if (dependency == null)
            {
                return HttpNotFound();
            }
            return View(dependency);
        }

        // POST: /Dependency/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Dependency dependency = db.Dependencies.Find(id);
            int taskId = dependency.ItemId;
            db.Dependencies.Remove(dependency);
            db.SaveChanges();
            return ShowIncome(taskId);
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
