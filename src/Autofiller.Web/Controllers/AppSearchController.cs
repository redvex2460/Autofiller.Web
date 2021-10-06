using Autofiller.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Autofiller.Web.Controllers
{
    public class AppSearchController : Controller
    {
        private DataManager DataManager => DataManager.GetInstance();
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Search(string searchquery)
        {
            var Apps = DataManager.Apps.Data.Where(app => app.AppId.ToString().Contains(searchquery) || app.Name.ToLower().Contains(searchquery.ToLower()));
            return View(await Apps.ToListAsync());
        }

        [HttpPost]
        public JsonResult AddToQueue(long appid)
        {
            var Response = "";
            if (DataManager.Apps.Data.Find(app => app.AppId == appid) == null)
            {
                Console.WriteLine($"Steam App with ID {appid} not found.");
                Response = $"Steam App with ID {appid} not found.";
            }
            var queueItem = DataManager.Queue.Data.Find(app => app.AppId == appid);

            if (queueItem != null)
            {
                Console.WriteLine($"Steam App with ID {appid} already queued");
                Response = $"Steam App with ID {appid} already queued";
            }
            else
            {
                var app = new Data.Database.QueuedApp(
                    DataManager.Apps.Data.Find(app => app.AppId == appid).Name,
                    "windows",
                    DateTime.Now,
                    appid);

                DataManager.Queue.Add(app);

                Response = $"Added Steam App {app.Name} with ID {app.AppId} for {app.Platform} to download queue.";
            }
            return Json(new { message = Response });
        }
    }
}
