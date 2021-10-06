using Autofiller.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofiller.Data.Commands
{
    public class StopDownloadCommand : ICommand<string>
    {
        DataManager DataManager => DataManager.GetInstance();
        public string Result { get; set; }

        public ICommand<string> Execute()
        {
            if (!DataManager.IsDownloading)
                Result = "There is no Download active";
            else
            {
                Result = DataManager.StopDownload();
            }
            return this;
        }

        //public string StartStopDownload()
        //{
        //    if (DownloadTask != null)
        //    {
        //        DownloadManager.StopDownload();
        //        DownloadManager = null;
        //        return "Download Canceled...";
        //    }
        //    if (DataManager.Queue.Data.Where(app => app.Status == DownloadStatus.Queued).Count() <= 0)
        //        return "Nothing to Download...";
        //    DownloadTask = Task.Run(Download);
        //    return "Download Started.";
        //}
    }
}
