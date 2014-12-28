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
    public class ProjectFileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /ProjectFile/
        public ActionResult Index()
        {
            var projectsfiles = db.ProjectsFiles.Include(p => p.Folder).Include(p => p.Project);
            return View(projectsfiles.ToList());
        }

        // GET: /ProjectFile/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectsFile projectsfile = db.ProjectsFiles.Find(id);
            if (projectsfile == null)
            {
                return HttpNotFound();
            }
            return View(projectsfile);
        }

        // GET: /ProjectFile/Create
        public ActionResult Create()
        {
            ViewBag.FolderId = new SelectList(db.ProjectsFolders, "Id", "Name");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            return View();
        }

        // POST: /ProjectFile/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,FolderId,ProjectId,FullPath,Name")] ProjectsFile projectsfile)
        {
            if (ModelState.IsValid)
            {
                db.ProjectsFiles.Add(projectsfile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FolderId = new SelectList(db.ProjectsFolders, "Id", "Name", projectsfile.FolderId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", projectsfile.ProjectId);
            return View(projectsfile);
        }

        // GET: /ProjectFile/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectsFile projectsfile = db.ProjectsFiles.Find(id);
            if (projectsfile == null)
            {
                return HttpNotFound();
            }
            ViewBag.FolderId = new SelectList(db.ProjectsFolders, "Id", "Name", projectsfile.FolderId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", projectsfile.ProjectId);
            return View(projectsfile);
        }

        // POST: /ProjectFile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,FolderId,ProjectId,FullPath,Name")] ProjectsFile projectsfile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projectsfile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FolderId = new SelectList(db.ProjectsFolders, "Id", "Name", projectsfile.FolderId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", projectsfile.ProjectId);
            return View(projectsfile);
        }

        // GET: /ProjectFile/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectsFile projectsfile = db.ProjectsFiles.Find(id);
            if (projectsfile == null)
            {
                return HttpNotFound();
            }
            return View(projectsfile);
        }

        // POST: /ProjectFile/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProjectsFile projectsfile = db.ProjectsFiles.Find(id);
            db.ProjectsFiles.Remove(projectsfile);
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
