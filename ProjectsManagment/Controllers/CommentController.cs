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
    public class CommentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public static IList<Comment> GetCommentsFor(ApplicationDbContext context, int tableId, int itemId, int count)
        {
            IQueryable<Comment> comments;
            if(count<0)
                comments = context.Comments.Where(com => com.TableId == tableId && com.ItemId == itemId).Select(c => c).OrderByDescending(com=>com.Date);
            else
                comments = context.Comments.Where(com => com.TableId == tableId && com.ItemId == itemId).Select(c => c).OrderByDescending(com => com.Date).Take(count);
            return comments.ToList();
        }
        /// <summary>
        /// This method doesn't execute SaveDb method!
        /// </summary>
        /// <param name="context"></param>
        /// <param name="text"></param>
        /// <param name="table"></param>
        /// <param name="itemId"></param>
        /// <param name="userName"></param>
        public static void AddComment(ApplicationDbContext context, string text, string table, int itemId, string userName)
        {
            AddComment(context, text, table, itemId, userName, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="text"></param>
        /// <param name="table"></param>
        /// <param name="itemId"></param>
        /// <param name="userName"></param>
        /// <param name="bad">Замечание если истина, иначе - комментарий</param>
        public static Comment AddComment(ApplicationDbContext context, string text, string table, int itemId, string userName, bool bad)
        {
            try
            {
                ApplicationUser user_ = context.Users.Where(u => u.UserName == userName).Select(user => user).First();
                return AddComment(context, text, table, itemId, user_, bad);
            }
            catch
            {
                return null;
            }
            
        }

        public static Comment AddComment(ApplicationDbContext context, string text, string table, int itemId, ApplicationUser user, bool bad)
        {
            Comment comment = new Comment();
            comment.Date = DateTime.Now;
            comment.ItemId = itemId;
            switch(table.ToString())
            {
                case "проекты":
                    comment.Project = context.Projects.Find(itemId);
                    break;
                case "стадии":
                    comment.Stage = context.Stages.Find(itemId);
                    break;
                case "разделы":
                    comment.Partition = context.Partitions.Find(itemId);
                    break;
                case "задания":
                    comment.Task = context.Tasks.Find(itemId);
                    break;
                case "файлы":
                    break;
            }
            comment.TableId = context.TableTypes.Where(tn => tn.TableName.ToLower() == table.ToLower()).Select(tt => tt.Id).First();
            comment.Text = text.Trim();
            comment.User = user;
            comment.Bad = bad;
            context.Comments.Add(comment);
            return comment;
        }

        public ActionResult ShowComment(Comment comment)
        {
            return PartialView("OneComment", comment);
        }

        public ActionResult ShowCommentById(int id)
        {
            Comment comment = db.Comments.Find(id);
            return PartialView("OneComment", comment);
        }



        // GET: /Comment/Create
        public ActionResult Create(int? tableId, int? itemId)
        {
            if (tableId == null || itemId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Comment comment = new Comment();
            comment.TableId = (int)tableId;
            comment.ItemId = (int)itemId;            
            return PartialView(comment);
        }

        // POST: /Comment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,TableId,ItemId,Text")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.User = db.Users.Where(u => u.UserName == User.Identity.Name).Select(user => user).First();
                comment.Date = DateTime.Now;
                db.Comments.Add(comment);
                db.SaveChanges();
                //убрать все ключи с itemId
                return PartialView("CommentSummary",GetCommentsFor(db,comment.TableId,comment.ItemId,-1));
            }
            return PartialView(comment);
        }

        // GET: /Comment/Edit/5
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return PartialView(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditView(int id)
        {
            Comment comment = db.Comments.Find(id);
            return PartialView("Edit", comment);
        }
        // POST: /Comment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, string text)
        {
            Comment comment = db.Comments.Find(id);
            if (ModelState.IsValid)
            {
                
                comment.Text = text;
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return PartialView("OneComment", comment);
            }
            return PartialView(comment);
        }

        // GET: /Comment/Delete/5
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            int tableId = comment.TableId;
            int itemId = comment.ItemId;
            db.Comments.Remove(comment);
            db.SaveChanges();
            return PartialView("CommentSummary", new { tableId = tableId, itemId = itemId, count = -1 });
        }

        // POST: /Comment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            if (comment == null)
                return HttpNotFound();
            int tableId = comment.TableId;
            int itemId = comment.ItemId;
            db.Comments.Remove(comment);
            db.SaveChanges();
            return PartialView("CommentSummary", GetCommentsFor(db, tableId, itemId, -1));
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
