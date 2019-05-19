using EventPlatform.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EventPlatform.Controllers
{
    public class EventController : Controller
    {
        [HttpGet]
        public IActionResult List(string option = "")
        {
            byte[] arr;
            bool isRoleSet = HttpContext.Session.TryGetValue("role", out arr);
            if (!(isRoleSet && Models.User.isNormalUser((UserType)HttpContext.Session.GetInt32("role"))))
            {
                return RedirectToAction("Index", "Profile");
            }

            if ((UserType)HttpContext.Session.GetInt32("role") == UserType.organizer)
            {
                ViewData["operationResponse"] = TempData["operationResponse"] == null ? null : (string)TempData["operationResponse"];
                ViewData["operationSucces"] = TempData["operationSucces"] == null ? false : (bool)TempData["operationSucces"];
                ViewData["EventList"] = Event.SelectListOrganizer(option, (int)HttpContext.Session.GetInt32("userid"));
                ViewData["role"] = HttpContext.Session.GetInt32("role");
                ViewData["userid"] = HttpContext.Session.GetInt32("userid");
                return View("~/Views/Shared/EventListView.cshtml");
            }

            ViewData["operationResponse"] = TempData["operationResponse"] == null ? null : (string)TempData["operationResponse"];
            ViewData["operationSucces"] = TempData["operationSucces"] == null ? false : (bool)TempData["operationSucces"];

            ViewData["EventList"] = Event.SelectList(option);
            ViewData["role"] = HttpContext.Session.GetInt32("role");
            ViewData["userid"] = HttpContext.Session.GetInt32("userid");
            return View("~/Views/Shared/EventListView.cshtml");
        }
        [HttpGet]
        public IActionResult Index(int eventId)
        {
            byte[] arr;
            bool isRoleSet = HttpContext.Session.TryGetValue("role", out arr);
            if (!(isRoleSet && Models.User.isNormalUser((UserType)HttpContext.Session.GetInt32("role"))))
            {
                return RedirectToAction("Index", "Profile");
            }

            ViewData["operationResponse"] = TempData["operationResponse"] == null ? null : (string)TempData["operationResponse"];
            ViewData["operationSucces"] = TempData["operationSucces"] == null ? false : (bool)TempData["operationSucces"];

            var e = Event.Select(eventId);
            var ru = Rating.Select(eventId, (int)HttpContext.Session.GetInt32("userid"));
            var r = Rating.GetRating(eventId);
            ViewData["Event"] = e;
            if (ru == null)
                ViewData["RatingUser"] = 0;
            else ViewData["RatingUser"] = ru.Score;
            ViewData["role"] = HttpContext.Session.GetInt32("role");

            ViewData["Rating"] = r;
            //Set duration
            ViewData["Organiser"] = EventPlatform.Models.User.getUser(e.User_id).Username;
            

            return View("~/Views/Shared/EventView.cshtml");
        }

        [HttpGet]
        public IActionResult AddEvent()
        {
            ViewData["userid"] = (int)HttpContext.Session.GetInt32("userid");
            return View("~/Views/Shared/AddEventView.cshtml");
        }

        [HttpPost]
        public IActionResult Add(string annotation, string description, string name, DateTime date, IFormFile image, string place, float price, DateTime duration)
        {
            if (name == null || date == null || description == null || duration == null || image == null)
                return AddEvent();
            byte[] img;
            using (var ms = new System.IO.MemoryStream())
            {
                image.CopyTo(ms);
                img = ms.ToArray();
            }
            using (var db = new ModelContext())
            {
                if (name != null && date != null && description != null && duration != null && img != null)
                {
                    Event newEvent = new Event();
                    newEvent.Name = name;
                    newEvent.Date = date;
                    newEvent.Description = description;
                    newEvent.Duration = duration - date;
                    newEvent.Image = img;
                    newEvent.Price = price;
                    if (annotation != null)
                        newEvent.Annotation = annotation;
                    if (place != null)
                        newEvent.Place = place;
                    newEvent.User_id = (int)HttpContext.Session.GetInt32("userid");
                    newEvent.State = EventEnum.upcoming;
                    db.Add(newEvent);
                    db.SaveChanges();
                    return View("~/Views/Shared/Main.cshtml");
                }
                else
                {
                    return AddEvent();
                }
            }
        }

        [HttpPost]
        public IActionResult Remove(int eventId)
        {

            using (var db = new ModelContext())
            {
                Event evnt = new Event() { Id = eventId };
                var rating = db.Ratings.Where(r => r.Event_id == eventId);
                var schedule = db.Schedules.Where(s => s.Event_id == eventId);
                var tags = db.Tags.Where(t => t.Event_id == eventId);
                db.Tags.AttachRange(tags);
                db.Tags.RemoveRange(tags);
                db.SaveChanges();
                db.Ratings.AttachRange(rating);
                db.Ratings.RemoveRange(rating);
                db.SaveChanges();
                db.Schedules.AttachRange(schedule);
                db.Schedules.RemoveRange(schedule);
                db.SaveChanges();
                db.Events.Attach(evnt);
                db.Events.Remove(evnt);

                db.SaveChanges();
            }
            ViewData["role"] = HttpContext.Session.GetInt32("role");
            ViewData["userid"] = HttpContext.Session.GetInt32("userid");
            return View("~/Views/Shared/EventListView.cshtml");
        }

        [HttpPost]
        public IActionResult UpdateState(int eventId, int newState)
        {
            byte[] arr;
            bool isRoleSet = HttpContext.Session.TryGetValue("role", out arr);
            if (!(isRoleSet && ((UserType)HttpContext.Session.GetInt32("role") == UserType.organizer)))
            {
                return RedirectToAction("Index", "Profile");
            }

            using (var db = new ModelContext())
            {
                ViewData["ResponseMessage"] = Models.Event.Update(eventId, newState);
                
                var e = Event.Select(eventId);
                var ru = Rating.Select(eventId, (int)HttpContext.Session.GetInt32("userid"));
                var r = Rating.GetRating(eventId);
                ViewData["Event"] = e;
                if (ru == null)
                    ViewData["RatingUser"] = 0;
                else ViewData["RatingUser"] = ru.Score;
                ViewData["role"] = HttpContext.Session.GetInt32("role");

                ViewData["Rating"] = r;
            }
            

            return View("~/Views/Shared/EventView.cshtml");
        }
        [HttpGet]
        public IActionResult Edit(int eventId)
        {
            using (var db = new ModelContext())
            {
                var evnt = db.Events.Find(eventId);
                ViewData["Event"] = evnt;
            }

            return View("~/Views/Shared/AddEventView.cshtml");
        }

        [HttpPost]
        public IActionResult EditEvent(string name, string description, string annotation, DateTime date, string place, float price, DateTime duration, int eventId)
        {
            byte[] arr;
            bool isRoleSet = HttpContext.Session.TryGetValue("role", out arr);
            if (!(isRoleSet && ((UserType)HttpContext.Session.GetInt32("role") == UserType.organizer)))
            {
                return RedirectToAction("Index", "Profile");
            }

            using (var db = new ModelContext())
            {
                var evnt = db.Events.Find(eventId);
                evnt.Annotation = annotation;
                evnt.Date = date;
                evnt.Description = description;
                evnt.Duration = duration - date;
                evnt.Name = name;
                evnt.Place = place;
                evnt.Price = price;
                db.Update(evnt);
                db.SaveChanges();
                var e = Event.Select(eventId);
                var ru = Rating.Select(eventId, (int)HttpContext.Session.GetInt32("userid"));
                var r = Rating.GetRating(eventId);
                ViewData["Event"] = e;
                if (ru == null)
                    ViewData["RatingUser"] = 0;
                else ViewData["RatingUser"] = ru.Score;
                ViewData["role"] = HttpContext.Session.GetInt32("role");

                ViewData["Rating"] = r;
                //Set duration
                ViewData["Organiser"] = EventPlatform.Models.User.getUser(e.User_id).Username;
            }


            return View("~/Views/Shared/EventView.cshtml");
        }

    }
}
