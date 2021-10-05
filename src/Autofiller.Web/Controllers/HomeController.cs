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
        DataManager DataManager => DataManager.Instance;
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult RemoveFromQueue(long appid)
        {
            string Response = "";
            if (DataManager.Apps.Data.Find(app => app.AppId == appid) == null)
            {
                Console.WriteLine($"Steam App with ID {appid} not found.");
                Response = $"Steam App with ID {appid} not found.";
            }
                var queueItem = DataManager.Queue.Data.Find(app => app.AppId == appid);

            if (queueItem == null)
            {
                Console.WriteLine($"Steam App with ID {appid} is not queued");
                Response = $"Steam App with ID {appid} is not queued";
            }
            else
            {
                var app = DataManager.Queue.Data.Find(item => item.AppId == appid);
                DataManager.Queue.Remove(app);

                Response = $"Removed Steam App {app.Name} with ID {app.AppId} for {app.Platform} from download queue.";
            }
            return Json(new { message = Response });
        }

        [HttpPost]
        public JsonResult StartDownload()
        {
            return Json(new { message = DataManager.Instance.SteamWrapper.StartDownload() });
        }

        public JsonResult GetDownloadProgress()
        {
            var dlmgr = DataManager.SteamWrapper.DownloadManager;
            if (dlmgr == null)
                return Json(new { game = "", action = "", progress = 0 });
            return Json(new { game = dlmgr.Game, action = dlmgr.Action, progress = dlmgr.Progress });
        }
    }
}