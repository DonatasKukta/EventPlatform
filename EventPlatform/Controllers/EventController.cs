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
        public IActionResult Add(string annotation, string description, string name, DateTime date, IFormFile image, string place, float price, DateTime duration, string ageTag, string placeTag, string typeTag)
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
                    var userId = (int) HttpContext.Session.GetInt32("userid");

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
                    newEvent.User_id = userId;
                    newEvent.State = EventEnum.upcoming;
                    Event.Insert(newEvent);

                    // *** Tag management for promotions ***
                    var newTypeTag = new Tag
                    {
                        Name = typeTag,
                        Event_id = newEvent.Id,
                        User_id = userId,
                        Weight = -1
                    };
                    var newPlaceTag = new Tag
                    {
                        Name = placeTag,
                        Event_id = newEvent.Id,
                        User_id = userId,
                        Weight = -1
                    };
                    var newAgeTag = new Tag
                    {
                        Name = ageTag,
                        Event_id = newEvent.Id,
                        User_id = userId,
                        Weight = -1
                    };

                    Tag.Insert(newAgeTag);
                    Tag.Insert(newPlaceTag);
                    Tag.Insert(newTypeTag);

                    // ***

                    return List();
                }

                return AddEvent();
            }
        }

        [HttpPost]
        public IActionResult Remove(int eventId)
        {

            Event evnt = new Event() { Id = eventId };

            Models.Event.Delete(evnt);
           
            ViewData["role"] = HttpContext.Session.GetInt32("role");
            ViewData["userid"] = HttpContext.Session.GetInt32("userid");
            
            return List();
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

                //Get tags
                var allTags = Tag.SelectList();
                var eventTags = allTags.Where(x => x.Event_id == evnt.Id).OrderBy(x=>x.Name).ToList();

                switch (eventTags.Count)
                {
                    case 0:
                        ViewData["TagOne"] = "-";
                        ViewData["TagTwo"] = "-";
                        ViewData["TagThree"] = "-";
                        break;
                    case 1:
                        ViewData["TagOne"] = eventTags[0].GetNameValue();
                        ViewData["TagTwo"] = "-";
                        ViewData["TagThree"] = "-";
                        break;
                    case 2:
                        ViewData["TagOne"] = eventTags[0].GetNameValue();
                        ViewData["TagTwo"] = eventTags[1].GetNameValue();
                        ViewData["TagThree"] = "-";
                        break;
                    case 3:
                        ViewData["TagOne"] = eventTags[0].GetNameValue();
                        ViewData["TagTwo"] = eventTags[1].GetNameValue();
                        ViewData["TagThree"] = eventTags[2].GetNameValue();
                        break;
                    default:
                        ViewData["TagOne"] = "-";
                        ViewData["TagTwo"] = "-";
                        ViewData["TagThree"] = "-";
                        break;
                }
            }

            return View("~/Views/Shared/AddEventView.cshtml");
        }

        [HttpPost]
        public IActionResult EditEvent(string name, string description, string annotation, DateTime date, string place, float price, DateTime duration, int eventId, string ageTag, string placeTag, string typeTag)
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
                Event.Update(evnt);
                
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
                ViewData["Organiser"] = Models.User.getUser(e.User_id).Username;

                var allTags = Tag.SelectList();
                var eventTags = allTags.Where(x => x.Event_id == evnt.Id).OrderBy(x=>x.Name).ToList();
           
                if (eventTags.Any())
                {
                    switch (eventTags.Count)
                    {
                        case 1:
                            eventTags[0].Name = typeTag != "acurrent" && !eventTags.Exists(x => x.Name == typeTag)
                                ? typeTag
                                : eventTags[0].Name;

                            Tag.Update(eventTags[0]);

                            break;
                        case 2:
                            eventTags[0].Name = typeTag != "acurrent" && !eventTags.Exists(x => x.Name == typeTag)
                                ? typeTag
                                : eventTags[0].Name;
                            eventTags[1].Name = placeTag != "bcurrent" && !eventTags.Exists(x => x.Name == placeTag)
                                ? placeTag
                                : eventTags[1].Name;

                            Tag.Update(eventTags[0]);
                            Tag.Update(eventTags[1]);

                            break;
                        case 3:
                            eventTags[0].Name = typeTag != "acurrent" && !eventTags.Exists(x => x.Name == typeTag)
                                ? typeTag
                                : eventTags[0].Name;
                            eventTags[1].Name = placeTag != "bcurrent" && !eventTags.Exists(x => x.Name == placeTag)
                                ? placeTag
                                : eventTags[1].Name;
                            eventTags[2].Name = ageTag != "ccurrent" && !eventTags.Exists(x => x.Name == ageTag)
                                ? ageTag
                                : eventTags[2].Name;

                            Tag.Update(eventTags[0]);
                            Tag.Update(eventTags[1]);
                            Tag.Update(eventTags[2]);

                            break;
                        default:
                            eventTags.ForEach(Tag.Remove);

                            var tagOneNew = new Tag
                            {
                                Name = typeTag,
                                Event_id = eventId,
                                User_id = e.User_id,
                                Weight = -1,
                            };

                            var tagTwoNew = new Tag
                            {
                                Name = placeTag,
                                Event_id = eventId,
                                User_id = e.User_id,
                                Weight = -1
                            };

                            var tagThreeNew = new Tag
                            {
                                Name = ageTag,
                                Event_id = eventId,
                                User_id = e.User_id,
                                Weight = -1
                            };

                            Tag.Insert(tagOneNew);
                            Tag.Insert(tagTwoNew);
                            Tag.Insert(tagThreeNew);
                            break;
                    }
                }

                else
                {
                    var tagOneNew = new Tag
                    {
                        Name = typeTag,
                        Event_id = eventId,
                        User_id = e.User_id,
                        Weight = -1
                    };
                    var tagTwoNew = new Tag
                    {
                        Name = placeTag,
                        Event_id = eventId,
                        User_id = e.User_id,
                        Weight = -1
                    };
                    var tagThreeNew = new Tag
                    {
                        Name = ageTag,
                        Event_id = eventId,
                        User_id = e.User_id,
                        Weight = -1
                    };

                    Tag.Insert(tagOneNew);
                    Tag.Insert(tagTwoNew);
                    Tag.Insert(tagThreeNew);
                }
            }


            return View("~/Views/Shared/EventView.cshtml");
        }

    }
}
