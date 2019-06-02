using System;
using System.Collections.Generic;
using System.Linq;
using EventPlatform.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EventPlatform.Controllers
{
    public class PromotionController : Controller
    {
        [HttpGet]
        public IActionResult GetList(string tagName = "visi")
        {
            //Get user id
            var userId = (int)HttpContext.Session.GetInt32("userid");
            var profileController = new ProfileController();
            
            //Get user interests
            var userInterests = profileController.GetUserInterests(userId);
            //Get all promotions and select only approved
            var allPromotions = Models.Promotion.SellectList();
            var approvedPromotionsList = allPromotions.Where(x => x.State == OrderState.approved).ToList();

            //Get all events
            var allEvents = Event.SelectList("");

            //Get all tags
            var allTags = Tag.SelectList();

            //Get specified number for favourite event tags of selected user 
            List<string> topUserTags;

            //*****
            //IF USER HAS CURRENT TAGS
            //*****
            //If user has interests - evaluate interests and take specific promotions
            if (userInterests.Count != 0)
            {
                topUserTags = EvaluateInterests(userInterests);

                //Select event tags that have same name value as users favourite ones
                var userFavouriteTags = allTags.Where(x => topUserTags.Contains(x.Name) && x.Weight == -1).ToList();

                //Filter selected tags that have same name as user fav and have approved promotions with their events
                var appPromEventsByTag = allEvents.Where(
                    x => approvedPromotionsList.Exists(y => y.Event_id == x.Id) &&
                         userFavouriteTags.Exists(z => z.Event_id == x.Id)).ToList();

                //Form promotions list from promotions that have their events in formed list
                var promotionsFinalList = approvedPromotionsList
                    .Where(x => appPromEventsByTag.Exists(y => y.Id == x.Event_id)).ToList();

                //If filter is selected - select only promotions with included events that have selected tag value
                if (tagName != "visi")
                {
                    var filteredTags = Tag.SelectList();
                    filteredTags = filteredTags.Where(x => x.Name == tagName).ToList();

                    appPromEventsByTag = appPromEventsByTag.Where(x => filteredTags.Exists(y => y.Event_id == x.Id))
                        .ToList();

                    promotionsFinalList = promotionsFinalList
                        .Where(x => appPromEventsByTag.Exists(y => y.Id == x.Event_id)).ToList();
                }

                //Form dictionary for image loading
                var imageDictionary = new Dictionary<int, string>();

                //Cycle through promotions and add formed line values to dictionary
                foreach (var promotion in promotionsFinalList)
                {
                    var base64Image = Convert.ToBase64String(promotion.Image);
                    var imagePathString = $"data:image/jpg;base64,{base64Image}";
                    imageDictionary.Add(promotion.Id, imagePathString);
                }

                //Bind formed data values
                ViewData["UserPromotions"] = promotionsFinalList;
                ViewData["UserEvents"] = appPromEventsByTag;
                ViewData["Images"] = imageDictionary;
            }

            //*****
            //IF USER HAS NO CURRENT TAGS
            //*****
            else
            {
                var promotionsByEvent = allEvents.Where(x => approvedPromotionsList.Exists(y => y.Event_id == x.Id)).ToList();

                //If filter is selected - select only promotions with included events that have selected tag value
                if (tagName != "visi")
                {
                    var filteredTags = allTags;
                    filteredTags = filteredTags.Where(x => x.Name == tagName).ToList();

                    promotionsByEvent = promotionsByEvent.Where(x => filteredTags.Exists(y => y.Event_id == x.Id))
                        .ToList();

                    approvedPromotionsList = approvedPromotionsList
                        .Where(x => promotionsByEvent.Exists(y => y.Id == x.Event_id)).ToList();
                }

                //Form dictionary for image loading
                var imageDictionary = new Dictionary<int, string>();

                //Cycle through promotions and add formed line values to dictionary
                foreach (var promotion in approvedPromotionsList)
                {
                    var base64Image = Convert.ToBase64String(promotion.Image);
                    var imagePathString = $"data:image/jpg;base64,{base64Image}";
                    imageDictionary.Add(promotion.Id, imagePathString);
                }

                //Bind formed data values
                ViewData["UserPromotions"] = approvedPromotionsList.OrderByDescending(x=>x.Id).ToList();
                ViewData["UserEvents"] = promotionsByEvent;
                ViewData["Images"] = imageDictionary;
            }

            ViewData["role"] = HttpContext.Session.GetInt32("role");
            return View("~/Views/Shared/PromotionListView.cshtml");
        }

        private List<string> EvaluateInterests(List<Tag> userInterests)
        {
            const int TopNCount = 2;
            var result = new List<string>();

            //If user interests does not exeed selected const, add only specified amount of tags
            if (userInterests.Count > TopNCount)
            {
                for (var i = 0; i < TopNCount; i++)
                {
                    //create new tag for each input line with weight 0
                    var maxTag = new Tag { Weight = 0 };

                    //Cycle through all interests and find one with largest weight
                    foreach (var userInterest in userInterests)
                    {
                        if (userInterest.Weight >= maxTag.Weight)
                        {
                            maxTag = userInterest;
                        }
                    }

                    //Add tag to new list and remove from current
                    result.Add(maxTag.Name);
                    userInterests.Remove(maxTag);
                }
            }
            //Else add all user interests to list
            else
            {
                //Else add all tags
                foreach (var userInterest in userInterests)
                {
                    result.Add(userInterest.Name);
                }
            }

            return result;
        }

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
