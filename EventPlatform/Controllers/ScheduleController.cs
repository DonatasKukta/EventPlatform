using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventPlatform.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EventPlatform.Controllers
{
    public class ScheduleController : Controller
    {
        [HttpGet]
        public IActionResult List(string option = "")
        {
            var userId = HttpContext.Session.GetInt32("userid");

            ViewData["operationResponse"] = TempData["operationResponse"] == null ? null : (string)TempData["operationResponse"];
            ViewData["operationSucces"] = TempData["operationSucces"] == null ? false : (bool)TempData["operationSucces"];

            var scheduleItems = Schedule.Select(userId.Value);
            var eventIds = new List<int>();

            var daysInCurrentMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            var calendarLayout = new Dictionary<int,int>();

            foreach (var scheduleItem in scheduleItems)
            {
                eventIds.Add(scheduleItem.Event_id);
            }

            var events = Schedule.SelectSheduleEvents(eventIds);

            for (int i = 0; i < daysInCurrentMonth; i++)
            {
                var dayEventList = new List<Event>();
                foreach (var @event in events)
                {
                    if (@event.Date.Year == DateTime.Now.Year && @event.Date.Month == DateTime.Now.Month && @event.Date.Day == i+1)
                    {
                        dayEventList.Add(@event);
                    }
                }
                var dayEventCount = dayEventList.Count+1;

                calendarLayout.Add(i+1, dayEventCount);
            }

            ViewData["calendarLayout"] = calendarLayout;
            ViewData["month"] = DateTime.Now.ToString("MMMM").ToUpper();
            ViewData["EventList"] =  events;
            ViewData["role"] = HttpContext.Session.GetInt32("role");
            ViewData["userid"] = HttpContext.Session.GetInt32("userid");
            return View("~/Views/Shared/ScheduleView.cshtml");
        }

        [HttpPost]
        public IActionResult Remove(int eventId)
        {
            var userId = HttpContext.Session.GetInt32("userid");

            ViewData["role"] = HttpContext.Session.GetInt32("role");
            ViewData["userid"] = userId;

            if (userId == null || eventId == 0)
            {
                return List();
            }

            Schedule.Delete(eventId, userId.Value);

            return List();
        }

        [HttpPost]
        public IActionResult AddToUserSchedule(int eventId, string redirectAction)
        {
            byte[] arr;
            var userId = HttpContext.Session.GetInt32("userid");
            bool isRoleSet = HttpContext.Session.TryGetValue("role", out arr);
            if (!(isRoleSet && Models.User.isNormalUser((UserType)HttpContext.Session.GetInt32("role"))))
            {
                return RedirectToAction("Index", "Profile");
            }

            var insertionResult = Models.Schedule.Insert(eventId, (int) HttpContext.Session.GetInt32("userid"));
            TempData["operationResponse"] = insertionResult.Item1;
            TempData["operationSucces"] = insertionResult.Item2;

            var allTags = Tag.SelectList();
            //Select tags ment for event
            var selectedEventTags = allTags.Where(x => x.Event_id == eventId && x.Weight == -1).ToList();
            //Select current user tags
            var userTags = allTags.Where(x => x.User_id == userId && x.Weight != -1).ToList();

            if (userId != null)
            {
                foreach (var selectedEventTag in selectedEventTags)
                {
                    //Check if there is such tag name
                    if (userTags.Exists(x=>x.Name == selectedEventTag.Name))
                    {
                        //If there is - select and update weight
                        var tagToUpdate = userTags.First(x => x.Name == selectedEventTag.Name);
                        tagToUpdate.Weight++;
                        Tag.Update(tagToUpdate);
                        continue;
                    }
                    
                    //Otherwise just add as new tag
                    var newUserTag1 = new Tag
                    {
                        Event_id = selectedEventTag.Event_id,
                        Name = selectedEventTag.Name,
                        User_id = userId.Value,
                        Weight = 1
                    };
                    Tag.Insert(newUserTag1);
                }
            }


            if (redirectAction.Equals("EventView"))
            {
                return RedirectToAction("Index", "Event", new { eventId = eventId });
            }
            else if (redirectAction.Equals("EventListView"))
            {
                return RedirectToAction("List", "Event");
            }
            else //Default action....
            {
                return RedirectToAction("Index", "Event", new { eventId = eventId });
            }
        }
    }
}
