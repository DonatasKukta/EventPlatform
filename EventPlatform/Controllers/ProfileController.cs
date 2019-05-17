using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var users = Models.User.getUserList();
            foreach (var usr in users)
            {
                ViewData["LoggedInUsers"] += usr.Username + "\n";
            }
            ViewData["LoggedInUsersCount"] = users.Count;
            ViewData["Role"] = Models.User.getType(userObj.Type);
            ViewData["Username"] = username;
            ViewData["Title"] = "Main page";

            HttpContext.Session.SetInt32("role",(int) userObj.Type);
            HttpContext.Session.SetInt32("userid", userObj.Id);
            
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
            }
            ViewData["Title"] = "Login page";
            return View("~/Views/Shared/Login.cshtml");
        }
    }
}