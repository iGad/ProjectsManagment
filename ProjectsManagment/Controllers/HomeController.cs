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
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Tasks()
        {
            IEnumerable<Task> tasks = db.Tasks.Where(t => t.Executor.UserName == User.Identity.Name).Select(task => task).ToList();
           
            return View(tasks);
        }

        public ActionResult ShowFullTask(Task task)
        {
            return PartialView("FullTaskInfo", task);
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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
