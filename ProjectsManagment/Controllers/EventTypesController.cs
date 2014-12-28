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
     [Authorize(Roles = "Администратор, Директор")]
    public class EventTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /EventTypes/
        public ActionResult Index()
        {
            
            return View(db.EventsTypes.ToList());
        }


        public static int GetEventTypeId(ApplicationDbContext context, string eventTypeName)
        {
            var tts = context.EventsTypes.Where(t => t.Name.ToLower() == eventTypeName.ToLower()).Select(tt => tt);
            if (tts == null || tts.Count() == 0)
            {
                EventsType tt = new EventsType();
                tt.Name = eventTypeName;
                int id = context.EventsTypes.Add(tt).Id;
                ProjectController.SaveDB(context);
                return id;
            }
            else
                return tts.First().Id;
        }

        public static EventsType GetEventType(ApplicationDbContext context, string eventTypeName)
        {
            var tts = context.EventsTypes.Where(t => t.Name.ToLower() == eventTypeName.ToLower()).Select(tt => tt);
            if (tts == null || tts.Count() == 0)
            {
                EventsType tt = new EventsType();
                tt.Name = eventTypeName;
                context.EventsTypes.Add(tt);
                ProjectController.SaveDB(context);
                return tt;
            }
            else
                return tts.First();
        }
        // GET: /EventTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /EventTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Name")] EventsType eventstype)
        {
            if (ModelState.IsValid)
            {
                db.EventsTypes.Add(eventstype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(eventstype);
        }

        // GET: /EventTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventsType eventstype = db.EventsTypes.Find(id);
            if (eventstype == null)
            {
                return HttpNotFound();
            }
            return View(eventstype);
        }

        // POST: /EventTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Name")] EventsType eventstype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventstype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(eventstype);
        }

        // GET: /EventTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventsType eventstype = db.EventsTypes.Find(id);
            if (eventstype == null)
            {
                return HttpNotFound();
            }
            return View(eventstype);
        }

        // POST: /EventTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EventsType eventstype = db.EventsTypes.Find(id);
            db.EventsTypes.Remove(eventstype);
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
