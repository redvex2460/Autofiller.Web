using Autofiller.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofiller.Data.Commands
{
    public class StartDownloadCommand : ICommand<string>
    {
        DataManager DataManager => DataManager.GetInstance();
        public string Result { get; set; }

        public ICommand<string> Execute()
        {
            if (DataManager.Queue.Data.Where(app => app.Status == DownloadStatus.Queued).Count() <= 0)
                Result = "Nothing to Download...";
            else if(DataManager.AuthorisedUsers.Data.Count <= 0)
                Result = "No authorised users, please navigate to \"Settings\" and authorise your steam-account";
            else
            {
                DataManager.StartDownload();
                Result = "Download Started.";
            }
            return this;
        }
    }
}
