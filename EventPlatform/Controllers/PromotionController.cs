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
                if (((UserType)HttpContext.Session.GetInt32("role") == UserType.organizer))
                {
                    ViewData["PromotionList"] = Models.Promotion.SelectList(option, (int)HttpContext.Session.GetInt32("userid"));
                    ViewData["role"] = HttpContext.Session.GetInt32("role");
                    return View("~/Views/Shared/PromotionListView.cshtml");
                }
                return RedirectToAction("Index", "Profile");
            }

            ViewData["PromotionList"] = Models.Promotion.SelectList(option, (int)HttpContext.Session.GetInt32("role"));
            ViewData["role"] = HttpContext.Session.GetInt32("role");
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
        public IActionResult Add(IFormFile image, string annotation, DateTime date, int eventId)
        {
            byte[] img;
            if (image == null || annotation == null || date == null)
                return AddPromotion();
            using (var ms = new System.IO.MemoryStream())
            {
                image.CopyTo(ms);
                img = ms.ToArray();
            }
            using (var db = new ModelContext())
            {
                if (date != null && annotation != null && img != null)
                {

                    Promotion newPromotion = new Promotion();

                    newPromotion.Date = date;
                    newPromotion.Image = img;
                    newPromotion.Annotation = annotation;
                    newPromotion.User_id = (int)HttpContext.Session.GetInt32("userid");
                    newPromotion.State = OrderState.waitingApproval;
                    newPromotion.Event_id = eventId;
                    Models.Promotion.Insert(newPromotion);
                    return List();
                }
                else
                {
                    return AddPromotion();
                }
            }
        }

        [HttpGet]
        public IActionResult AddPromotion()
        {
            ViewData["userid"] = (int)HttpContext.Session.GetInt32("userid");
            using (var db = new ModelContext())
            {
                ViewData["Events"] = db.Events.Where(s => s.User_id == (int)ViewData["userid"]).ToList();
            }
            return View("~/Views/Shared/AddPromotionView.cshtml");
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

        [HttpPost]
        public IActionResult Remove(int promotionId)
        {
            byte[] arr;
            bool isRoleSet = HttpContext.Session.TryGetValue("role", out arr);
            if (!(isRoleSet && ((UserType)HttpContext.Session.GetInt32("role") == UserType.organizer)))
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
                ViewData["ResponseMessage"] = Models.Promotion.Delete(promotionId);
            }
            return List();
        }



    }
}
