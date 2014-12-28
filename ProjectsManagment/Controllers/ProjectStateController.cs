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
    public class ProjectStateController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /ProjectState/
        public ActionResult Index()
        {
            return View(db.TasksStates.ToList());
        }

        public static TasksState GetState(ApplicationDbContext context, string stateName)
        {
            var tts = context.TasksStates.Where(t => t.Name.ToLower() == stateName.ToLower()).Select(tt => tt);
            if (tts == null)
            {
                TasksState tt = new TasksState();
                tt.Name = stateName;
                return context.TasksStates.Add(tt);
            }
            else
                return tts.First();
        }

        // GET: /ProjectState/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TasksState tasksstate = db.TasksStates.Find(id);
            if (tasksstate == null)
            {
                return HttpNotFound();
            }
            return View(tasksstate);
        }

        // GET: /ProjectState/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /ProjectState/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Name")] TasksState tasksstate)
        {
            if (ModelState.IsValid)
            {
                db.TasksStates.Add(tasksstate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tasksstate);
        }

        // GET: /ProjectState/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TasksState tasksstate = db.TasksStates.Find(id);
            if (tasksstate == null)
            {
                return HttpNotFound();
            }
            return View(tasksstate);
        }

        // POST: /ProjectState/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Name")] TasksState tasksstate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tasksstate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tasksstate);
        }

        // GET: /ProjectState/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TasksState tasksstate = db.TasksStates.Find(id);
            if (tasksstate == null)
            {
                return HttpNotFound();
            }
            return View(tasksstate);
        }

        // POST: /ProjectState/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TasksState tasksstate = db.TasksStates.Find(id);
            db.TasksStates.Remove(tasksstate);
            db.SaveChanges();
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
