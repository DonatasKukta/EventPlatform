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

            return View("~/Views/Shared/Main.cshtml");
        }
    }
}
