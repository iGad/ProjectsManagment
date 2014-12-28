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
    public class TableTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /TableTypes/
        public ActionResult Index()
        {
            return View(db.TableTypes.ToList());
        }

        public static int GetTableTypeId(ApplicationDbContext context, string tableName)
        {
            var tts = context.TableTypes.Where(t => t.TableName.ToLower() == tableName.ToLower()).Select(tt => tt);
            if (tts == null || tts.Count() == 0)
            {
                TablesType tt = new TablesType();
                tt.TableName = tableName;
                int id = context.TableTypes.Add(tt).Id;
                ProjectController.SaveDB(context);
                return id;
            }
            else
                return tts.First().Id;
        }

        public static TablesType GetTableType(ApplicationDbContext context, string tableName)
        {
            var tts = context.TableTypes.Where(t => t.TableName.ToLower() == tableName.ToLower()).Select(tt => tt);
            if (tts == null || tts.Count() == 0)
            {
                TablesType tt = new TablesType();
                tt.TableName = tableName;
                context.TableTypes.Add(tt);
                ProjectController.SaveDB(context);
                return tt;
            }
            else
                return tts.First();
        }

        // GET: /TableTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /TableTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,TableName")] TablesType tablestype)
        {
            if (ModelState.IsValid)
            {
                db.TableTypes.Add(tablestype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tablestype);
        }

        // GET: /TableTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TablesType tablestype = db.TableTypes.Find(id);
            if (tablestype == null)
            {
                return HttpNotFound();
            }
            return View(tablestype);
        }

        // POST: /TableTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,TableName")] TablesType tablestype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tablestype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tablestype);
        }

        // GET: /TableTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TablesType tablestype = db.TableTypes.Find(id);
            if (tablestype == null)
            {
                return HttpNotFound();
            }
            return View(tablestype);
        }

        // POST: /TableTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TablesType tablestype = db.TableTypes.Find(id);
            db.TableTypes.Remove(tablestype);
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
