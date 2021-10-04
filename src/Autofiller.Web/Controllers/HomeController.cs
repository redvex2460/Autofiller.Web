using Autofiller.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Autofiller.Web.Controllers
{
    public class HomeController : Controller
    {
        DataManager dataManager => DataManager.Instance;
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult RemoveFromQueue(long appid)
        {
            string Response = "";
            if (dataManager.Apps.Data.Find(app => app.AppId == appid) == null)
            {
                Console.WriteLine($"Steam App with ID {appid} not found.");
                Response = $"Steam App with ID {appid} not found.";
            }
                var queueItem = dataManager.Queue.Data.Find(app => app.AppId == appid);

            if (queueItem == null)
            {
                Console.WriteLine($"Steam App with ID {appid} is not queued");
                Response = $"Steam App with ID {appid} is not queued";
            }
            else
            {
                var app = dataManager.Queue.Data.Find(item => item.AppId == appid);
                dataManager.Queue.Remove(app);

                Response = $"Removed Steam App {app.Name} with ID {app.AppId} for {app.Platform} from download queue.";
            }
            return Json(new { message = Response });
        }

        [HttpPost]
        public JsonResult StartDownload()
        {
            return Json(new { message = DataManager.Instance.SteamWrapper.StartDownload() });
        }
    }
}