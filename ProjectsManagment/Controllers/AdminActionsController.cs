using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectsManagment.Controllers
{
    [Authorize(Roles = "Администратор, Директор")]
    public class AdminActionsController : Controller
    {
        //
        // GET: /AdminActions/
        public ActionResult Index()
        {
            return View("Index");
        }
	}
}