using Autofiller.Data.Database;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using Autofiller.Data.Commands;

namespace Autofiller.Data.Steam.Models
{
    public class DownloadManager
    {
        public DownloadManagerStatus Status { get; private set; }
        private SteamCommand _steamCommand;
        private readonly List<QueuedApp> _downloadQueue;
        public Thread DownloadThread;
        private bool shouldDownload = false;

        private DataManager DataManager => DataManager.GetInstance();

        public DownloadManager(Queue queue)
        {
            _downloadQueue = queue.Data.Where(item => item.Status == DownloadStatus.Queued).ToList();
        }

        public void StartDownload()
        {
            if (DownloadThread != null)
                return;
            shouldDownload = true;
            DownloadThread = new Thread(DownloadThreadAsync);
            DownloadThread.Start();
        }
        public void StopDownload()
        {
            if (DownloadThread == null)
                return;
            shouldDownload = false;
            new KillDownloadsCommand().Execute();
            DownloadThread = null;
        }

        private void DownloadThreadAsync()
        {
            var users = new GetAuthorisedUsersCommand().Execute().Result;
            if (users.Count <= 0 || !shouldDownload)
                return;
            foreach (var app in _downloadQueue)
            {
                foreach (var user in users)
                {
                    if (!shouldDownload)
                        return;
                    Status = new DownloadManagerStatus() { Game = app.Name };
                    _steamCommand = SteamCommand.Init();
                    _steamCommand.MessageRead += HandleSteamOutput;
                    _steamCommand
                    .Login(user)
                    .SetPlatform(Enum.Parse<SteamPlatforms>(app.Platform))
                    .SetDirectory(DataManager.Settings.DownloadDirectory)
                    .Update(app.AppId)
                    .Execute();
                    if (_steamCommand.Process.ExitCode != 0 || !shouldDownload)
                    {
                        return;
                    }
                        DataManager.Queue.Data.Find(item => item.AppId == app.AppId).Update();
                        DataManager.Queue.Data.Find(item => item.AppId == app.AppId).Status = DownloadStatus.Completed;
                    Thread.Sleep(2000);
                    new DeleteDownloadedFilesCommand().Execute();
                }
            }
            Status = null;
            DownloadThread = null;
        }

        private void HandleSteamOutput(string message)
        {
            if (message == null)
                return;
            try
            {
                string pattern = @"Update state \(0x.*?\) (?<ProgressType>.*), progress: (?<Progress>.*)? \((?<crntBit>\d*) \/ (?<maxBit>\d*)\)";
                var matches = Regex.Match(message, pattern);
                if (matches.Groups.Count >= 2)
                {
                    Status.Action = matches.Groups["ProgressType"].Value;
                    Status.Progress = double.Parse(matches.Groups["Progress"].Value);
                    Status.CurrentBit = double.Parse(matches.Groups["crntBit"].Value);
                    Status.MaximumBit = double.Parse(matches.Groups["maxBit"].Value);
                    Status.ChangeTime = DateTime.Now;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                StopDownload();
            }
        }
    }
}
