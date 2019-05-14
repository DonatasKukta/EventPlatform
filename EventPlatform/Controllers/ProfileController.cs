using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public IActionResult Login(string username, string password, string role)
        {
            ViewData["Username"] = username;
            ViewData["Role"] = role;

            //norint kreiptis i DB reikia sukurti konteksta sitaip
            using (var db = new Models.ModelContext())
            {
                if (username != null && password != null)
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
                    db.Add(user); //pridedam elementa i db
                    db.SaveChanges(); //pridejus reikia daryti saveChanges
                }
                //sitaip selectinam visus lenteles elementus
                foreach (var usr in db.Users)
                {
                    ViewData["LoggedInUsers"] += usr.Username + "\n";
                }
                ViewData["LoggedInUsersCount"] = db.Users.Count();
            }

            return View("~/Views/Shared/Main.cshtml");
        }
    }
}
