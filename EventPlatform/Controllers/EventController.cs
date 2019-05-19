using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EventPlatform.Models;
using Microsoft.AspNetCore.Http;
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

            ViewData["operationResponse"]= TempData["operationResponse"] == null ? null : (string)TempData["operationResponse"];
            ViewData["operationSucces"] = TempData["operationSucces"] == null ? false : (bool)TempData["operationSucces"];

            ViewData["EventList"] = Event.SelectList(option);
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
            ViewData["RatingUser"] = ru;
            ViewData["Rating"] = r;
            //Set duration
            ViewData["Organiser"] = EventPlatform.Models.User.getUser(e.User_id).Username;
            return View("~/Views/Shared/EventView.cshtml");
        }
    }
}
