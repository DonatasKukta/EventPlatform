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
        [HttpPost]
        public IActionResult AddToUserSchedule(int eventId, string redirectAction)
        {
            byte[] arr;
            bool isRoleSet = HttpContext.Session.TryGetValue("role", out arr);
            if (!(isRoleSet && Models.User.isNormalUser((UserType)HttpContext.Session.GetInt32("role"))))
            {
                throw new UnauthorizedAccessException("Vartotojui prieiga nesuteikta");
            }

            var insertionResult = Models.Schedule.Insert(eventId, (int) HttpContext.Session.GetInt32("userid"));
            TempData["operationResponse"] = insertionResult.Item1;
            TempData["operationSucces"] = insertionResult.Item2;

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
