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
    public class PromotionController : Controller
    {
        [HttpGet]
        public IActionResult List(int option = -1)
        {
            byte[] arr;
            bool isRoleSet = HttpContext.Session.TryGetValue("role", out arr);
            if (!(isRoleSet && ((UserType)HttpContext.Session.GetInt32("role") == UserType.admin)))
            {
                return RedirectToAction("Index", "Profile");
            }

            ViewData["PromotionList"] = Models.Promotion.SelectList(option);
            return View("~/Views/Administrator/PromotionListView.cshtml");
        }

        [HttpGet]
        public IActionResult Promotion(int promotionId)
        {
            byte[] arr;
            bool isRoleSet = HttpContext.Session.TryGetValue("role", out arr);
            if (!(isRoleSet && ((UserType)HttpContext.Session.GetInt32("role") == UserType.admin)))
            {
                return RedirectToAction("Index", "Profile");
            }
            if (promotionId < 0)
            {
                ViewData["Promotion"] = null;
                ViewData["organiserName"] = string.Empty;
                ViewData["eventName"] = string.Empty;
                ViewData["promotionState"] = string.Empty;
                ViewData["ResponseMessage"] = "Atleiskite, įvyko klaida.";
            }
            else
            {
                var tupleObj = Models.Promotion.Select(promotionId);
                ViewData["Promotion"] = tupleObj.Item1;
                ViewData["organiserName"] = tupleObj.Item2;
                ViewData["eventName"] = tupleObj.Item3;
                ViewData["promotionState"] = Models.Promotion.getStateString(tupleObj.Item1.State);
            }

            ViewData["ResponseMessage"] = null;
            return View("~/Views/Administrator/PromotionView.cshtml");
        }

        [HttpPost]
        public IActionResult UpdateState(int promotionId, int newState)
        {
            byte[] arr;
            bool isRoleSet = HttpContext.Session.TryGetValue("role", out arr);
            if (!(isRoleSet && ((UserType)HttpContext.Session.GetInt32("role") == UserType.admin)))
            {
                return RedirectToAction("Index", "Profile");
            }
            if (promotionId < 0)
            {
                ViewData["Promotion"] = null;
                ViewData["organiserName"] = string.Empty;
                ViewData["eventName"] = string.Empty;
                ViewData["ResponseMessage"] = "Atleiskite, įvyko klaida.";
                ViewData["promotionState"] = string.Empty;
            }
            else
            {
                ViewData["ResponseMessage"] = Models.Promotion.Update(promotionId, newState);
                var tupleObj = Models.Promotion.Select(promotionId);
                ViewData["Promotion"] = tupleObj.Item1;
                ViewData["organiserName"] = tupleObj.Item2;
                ViewData["eventName"] = tupleObj.Item3;
                ViewData["promotionState"] = Models.Promotion.getStateString(tupleObj.Item1.State);
            }
            return View("~/Views/Administrator/PromotionView.cshtml");
        }
    }
}
