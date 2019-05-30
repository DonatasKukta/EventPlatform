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
    public class ProfileController : Controller
    {
        // GET: /<controller>/
        [HttpGet]
        public IActionResult Index()
        {
            @ViewData["Title"] = "Login page";
            return View("~/Views/Shared/Login.cshtml");
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var userObj = Models.User.getUser(username);
            if (userObj == null)
            {
                ViewData["Title"] = "Login page";
                return View("~/Views/Shared/Login.cshtml");
            }
            ViewData["Role"] = Models.User.getType(userObj.Type);
            ViewData["Username"] = username;
            ViewData["Title"] = "Main page";

            HttpContext.Session.SetInt32("role",(int) userObj.Type);
            HttpContext.Session.SetInt32("userid", userObj.Id);
            
            return View("~/Views/Shared/Main.cshtml");
        }

        [HttpGet]
        public IActionResult Main()
        {
            byte[] arr;
            bool isRoleSet = HttpContext.Session.TryGetValue("role", out arr);
            if (!(isRoleSet && Models.User.isNormalUser((Models.UserType)HttpContext.Session.GetInt32("role"))))
            {
                return RedirectToAction("Index", "Profile");
            }
            ViewData["Role"] = Models.User.getType((Models.UserType)HttpContext.Session.GetInt32("role"));
            ViewData["Username"] = Models.User.getUser((int)HttpContext.Session.GetInt32("userid")).Username;

            return View("~/Views/Shared/Main.cshtml");
        }
           

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["Title"] = "Register page";
            return View("~/Views/Shared/Register.cshtml");
        }

        [HttpPost]
        public IActionResult Register(string email, string username, string password, string role)
        {
            //norint kreiptis i DB reikia sukurti konteksta sitaip
            using (var db = new Models.ModelContext())
            {
                if (email != null && username != null && password != null && role != null)
                {
                    Models.User user = new Models.User();
                    user.Username = username;
                    user.Password = password;
                    if (role == "admin")
                        user.Type = Models.UserType.admin;
                    else if (role == "organizer")
                        user.Type = Models.UserType.organizer;
                    else if (role == "participant")
                        user.Type = Models.UserType.participant;
                    else
                        return View("~/Views/Shared/Register.cshtml");

                    db.Add(user); //pridedam elementa i db
                    db.SaveChanges(); //pridejus reikia daryti saveChanges
                }
                else
                {
                    return View("~/Views/Shared/Register.cshtml");
                }
            }
            ViewData["Title"] = "Login page";
            return View("~/Views/Shared/Login.cshtml");
        }

        public List<Tag> GetUserInterests(int userId)
        {
            return Models.User.GetUserInterests(userId);
        }
    }
}