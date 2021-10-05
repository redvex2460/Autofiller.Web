using Autofiller.Data.Models;
using Autofiller.Data.Models.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Autofiller.Data.Steam.Models
{
    public class DownloadManager
    {
        public string Game => _app.Name;
        public string Action { get; set; }
        public double Progress { get; set; }
        public string User => _authorisedUser.UserName;
        private readonly QueuedApp _app;
        private readonly AuthorisedUser _authorisedUser;
        private SteamCommand _steamCommand;
        private DataManager DataManager => DataManager.Instance;

        public DownloadManager(QueuedApp app, AuthorisedUser user)
        {
            _app = app;
            _authorisedUser = user;
        }

        public void StartDownload()
        {
            _steamCommand = SteamCommand.Init();
            _steamCommand.MessageRead += HandleSteamOutput;
            _steamCommand
            .Login(_authorisedUser.UserName)
            .SetPlatform(Enum.Parse<SteamPlatforms>(_app.Platform))
            .SetDirectory(DataManager.Settings.DownloadDirectory)
            .Update(_app.AppId)
            .Execute();
        }

        private void HandleSteamOutput(string message)
        {
            if (message == null)
                return;
            try
            {
                string pattern = @"Update state \(0x..\) (?<ProgressType>.*), progress: (?<Progress>.*)? \(.*\)";
                var matches = Regex.Match(message, pattern);
                if (matches.Groups.Count >= 2)
                {
                    Action = matches.Groups["ProgressType"].Value;
                    Progress = double.Parse(matches.Groups["Progress"].Value);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                StopDownload();
            }
        }

        public void StopDownload()
        {
            _steamCommand.Kill();
        }
    }
}
