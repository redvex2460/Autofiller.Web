using Autofiller.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Autofiller.Web.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult AuthenticateUser(string username, string password, string guard)
        {
            var success = DataManager.Instance.SteamWrapper.Login(username, password, guard);
            var msg = "";
            if (success)
                msg = $"{username} successfully signed in.";
            else
                msg = $"Could not sign in {username}, please check your details and try again.";
            return Json(new { message = msg });
        }
    }
}
