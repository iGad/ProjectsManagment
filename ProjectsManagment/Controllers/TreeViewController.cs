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
    
    public class TreeViewController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //
        // GET: /TreeView/
        public ActionResult Index()
        {
            var projects = db.Projects.Select(p => p);
            return View(projects);
        }

        public ActionResult Collapse(int level, int id)
        {
            return PartialView("TreeViewItem");
        }

        public ActionResult Explore(int level, int id)
        {
            return PartialView("TreeViewItem");

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