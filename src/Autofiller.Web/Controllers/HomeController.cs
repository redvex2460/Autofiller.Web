using Autofiller.Data;
using Autofiller.Data.Commands;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Autofiller.Web.Controllers
{
    public class HomeController : Controller
    {
        DataManager DataManager => DataManager.GetInstance();
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
        public JsonResult StartStopDownload()
        {
            if(DataManager.IsDownloading)
                return Json(new { message = new StopDownloadCommand().Execute().Result });
            else
                return Json(new { message = new StartDownloadCommand().Execute().Result });
        }

        public JsonResult GetDownloadProgress()
        {
            if(DataManager.DownloadManager == null || DataManager.DownloadManager.Status == null)
                return Json(new { game = "", action = "", progress = 0 });
            var dlmgr = DataManager.DownloadManager.Status;
            return Json(new { game = dlmgr.Game, action = dlmgr.Action, progress = dlmgr.Progress, current = dlmgr.CurrentBit, max = dlmgr.MaximumBit, speed = dlmgr.DownloadSpeed });
        }
    }
}