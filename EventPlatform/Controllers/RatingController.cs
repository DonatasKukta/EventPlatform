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
    public class RatingController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddRating(int eventId, string redirectAction, int ratingValue)
        {
            byte[] arr;
            bool isRoleSet = HttpContext.Session.TryGetValue("role", out arr);
            if (!(isRoleSet && Models.User.isNormalUser((UserType)HttpContext.Session.GetInt32("role"))))
            {
                return RedirectToAction("Index", "Profile");
            }
            var inertionResult = Models.Rating.Insert(eventId, (int)HttpContext.Session.GetInt32("userid"), ratingValue);
            //var insertionResult = Models.Schedule.Insert(eventId, (int)HttpContext.Session.GetInt32("userid"));
            return RedirectToAction("Index", "Event", new { eventId = eventId });
        }
    }
}
