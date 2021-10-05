using Autofiller.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Autofiller.Web.Controllers
{
    public class AppSearchController : Controller
    {
        private DataManager dataManager => DataManager.Instance;
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Search(string searchquery)
        {
            var Apps = dataManager.Apps.Data.Where(app => app.AppId.ToString().Contains(searchquery) || app.Name.ToLower().Contains(searchquery.ToLower()));
            return View(Apps.ToList());
        }

        [HttpPost]
        public JsonResult AddToQueue(long appid)
        {
            var Response = "";
            if (dataManager.Apps.Data.Find(app => app.AppId == appid) == null)
            {
                Console.WriteLine($"Steam App with ID {appid} not found.");
                Response = $"Steam App with ID {appid} not found.";
            }
            var queueItem = dataManager.Queue.Data.Find(app => app.AppId == appid);

            if (queueItem != null)
            {
                Console.WriteLine($"Steam App with ID {appid} already queued");
                Response = $"Steam App with ID {appid} already queued";
            }
            else
            {
                var app = new Data.Models.Database.QueuedApp(
                    dataManager.Apps.Data.Find(app => app.AppId == appid).Name,
                    "windows",
                    DateTime.Now,
                    appid);

                dataManager.Queue.Add(app);

                Response = $"Added Steam App {app.Name} with ID {app.AppId} for {app.Platform} to download queue.";
            }
            return Json(new { message = Response });
        }
    }
}
