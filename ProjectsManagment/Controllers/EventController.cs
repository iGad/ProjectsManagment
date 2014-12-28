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
    public class EventController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //
        // GET: /Event/
        public ActionResult Index()
        {            
            return View();
        }

        public string GetEventsCount()
        {
            try
            {
                int count = db.EventsUsers.Where(e => e.User.UserName == User.Identity.Name && !e.Seen).Select(eu => eu).Count();
                if (count > 0)
                    return "(" + count.ToString() + ")";
                else
                    return "";
            }
            catch
            {
                return "";
            }
        }

        public ActionResult EventsSeen()
        {
            ApplicationUser user = db.Users.Where(u => u.UserName == User.Identity.Name).Select(us => us).First();
            IEnumerable<Event> events = GetEventsForUser(user.Id, true, false, 0);
            foreach (var e in events.ToList())
                MakeEventSeen(e.Id, user.Id);
            return Redirect("~/Event");//View( "~/Views/Event/Index.cshtml");
        }

        public string GetDescriptionForEvent(Event e)
        {
            string result = e.EventType.Name;
            
            if (e.EventComment != null)
            {
                result += ": \"" + e.EventComment.Text + "\" к ";
                if (e.EventComment.Task != null) result +=  "заданию <a href=\""+ Url.Action("Details","Task",new{ id = e.EventComment.Task.Id})+"\">" + e.EventComment.Task.Name + "</a>";
                if (e.EventComment.Partition != null) result += "разделу <a href=\"" + Url.Action("Details", "Partition", new { id = e.EventComment.Partition.Id }) + "\">" + e.EventComment.Partition.Name + "</a>";
                if (e.EventComment.Project != null) result += "проекту <a href=\"" + Url.Action("Details", "Project", new { id = e.EventComment.Project.Id }) + "\">" + e.EventComment.Project.Name + "</a>";
                if (e.EventComment.Stage != null) result += "стадии <a href=\"" + Url.Action("Details", "Stage", new { id = e.EventComment.Stage.Id }) + "\">" + e.EventComment.Stage.Name + "</a>";
                if (e.EventComment.File != null) result += "файлу " + e.EventComment.File.Name;
            }
            if (e.EventFile != null) result += " файл" + e.EventFile.Name;
            if (e.EventFolder != null) result += " папку" + e.EventFile.Name;
            if (e.EventTask != null) result += " задание <a href=\"" + Url.Action("Details", "Task", new { id = e.EventTask.Id }) + "\">" + e.EventTask.Name + "</a>";
            if (e.EventPartition != null) result += " раздел <a href=\"" + Url.Action("Details", "Partition", new { id = e.EventPartition.Id }) + "\">" + e.EventPartition.Name + "</a>";
            if (e.EventStage != null) result += " стадию <a href=\"" + Url.Action("Details", "Stage", new { id = e.EventStage.Id }) + "\">" + e.EventStage.Name + "</a>";
            if (e.EventProject != null) result += " проект <a href=\""+Url.Action("Details","Project",new{ id = e.EventProject.Id})+"\">" + e.EventProject.Name + "</a>";
            return result;
        }

        public ActionResult ShowEvents(bool showAll = false, bool seen = false, int count=20)
        {
            ApplicationUser user = db.Users.Where(u => u.UserName == User.Identity.Name).Select(us => us).First();
            IEnumerable<Event> events = GetEventsForUser(user.Id, showAll, seen, count);
           
                
            return PartialView("EventsView", events);
        }

        public static Event CreateEvent(ApplicationDbContext context, DateTime date, EventsType type, ApplicationUser user, DependencyTable tableName, object item)
        {
            Event e = new Event();
            e.EventType = type;
            e.Date = date;
            e.User = user;
            switch(tableName)
            #region
            {
                case DependencyTable.Task:
                    e.EventTask = item as Task;
                    e.ItemId = e.EventTask.Id;
                    e.Table = TableTypesController.GetTableType(context, "задания");
                    e.TableId = e.Table.Id;
                    break;
                case DependencyTable.Partition:
                    e.EventPartition = item as Partition;
                    e.ItemId = e.EventPartition.Id;
                    e.Table = TableTypesController.GetTableType(context, "разделы");
                    e.TableId = e.Table.Id;
                    break;
                case DependencyTable.Stage:
                    e.EventStage = item as Stage;
                    e.ItemId = e.EventStage.Id;
                    e.Table = TableTypesController.GetTableType(context, "стадии");
                    e.TableId = e.Table.Id;
                    break;
                case DependencyTable.Project:
                    e.EventProject = item as Project;
                    e.ItemId = e.EventProject.Id;
                    e.Table =  TableTypesController.GetTableType(context, "проекты");
                    e.TableId = e.Table.Id;
                    break;
                case DependencyTable.File:
                    e.EventFile = item as ProjectsFile;
                    e.ItemId = e.EventFile.Id;
                    e.Table = TableTypesController.GetTableType(context, "файлы");
                    e.TableId = e.Table.Id;
                    break;
                case DependencyTable.Comment:
                    e.EventComment = item as Comment;
                    e.ItemId = e.EventComment.Id;
                    e.Table = TableTypesController.GetTableType(context, "комментарии");
                    e.TableId = e.Table.Id;
                    break;
                case DependencyTable.User:
                    e.EventUser = item as ApplicationUser;
                    //e.ItemId = e.EventUser.Id;
                    e.Table = TableTypesController.GetTableType(context, "пользователи");
                    e.TableId = e.Table.Id;
                    break;
            }
            #endregion
            context.Events.Add(e);
            return e;
        }

        public static void CreateEventForUsers(ApplicationDbContext context, Event e, IEnumerable<ApplicationUser> users)
        {
            foreach(var user in users)
            {
                context.EventsUsers.Add(new EventsUsers(){EventId = e.Id, Event = e, User=user, Seen=false});
            }
            //ProjectController.SaveDB(context);
        }
        #region Сделать событие просмотренным
        public void MakeEventSeen(Event e, ApplicationUser user)
        {
            MakeEventSeen(e.Id, user.Id);
        }

        public void MakeEventSeen(int eventId, string userId)
        {
            EventsUsers eu = db.EventsUsers.Where(ev => ev.User.Id == userId && ev.EventId == eventId).Select(eus => eus).First();
            if (eu.Shown)
            {
                eu.Seen = true;
                db.Entry(eu).State = EntityState.Modified;
                ProjectController.SaveDB(db);
            }
        }
        public  void MakeEventSeen(Event e, string userId)
        {
            MakeEventSeen(e.Id, userId);
        }

        public  void MakeEventSeen(int eventId, ApplicationUser user)
        {
            MakeEventSeen(eventId, user.Id);
        }
#endregion
        public IEnumerable<Event> GetEventsForUser(string id, bool showAll, bool seen, int count)
        {
            IEnumerable<Event> result = null;
            if (!showAll)
            {
                try
                {
                    result = db.EventsUsers.Where(u => u.User.Id == id && u.Seen == seen).OrderByDescending(eu=>eu.Event.Date).Select(e => e.Event).Take(count);
                }
                catch
                {
                    result = null;
                }
            }
            else
            {
                try
                {
                    var all = db.EventsUsers.Where(u => u.User.Id == id && u.Seen == seen).OrderByDescending(eu => eu.Event.Date).Select(e => e);
                    foreach (EventsUsers eus in all)
                        if (!eus.Shown)
                            eus.Shown = true;
                    result = all.Select(e => e.Event);//db.EventsUsers.Where(u => u.User.Id == id && u.Seen == seen).Select(e => e.Event);
                }
                catch
                {
                    result = null;
                }
            }
            if (result != null)
            {
                
                return result.ToList();
            }
            else
                return null;
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