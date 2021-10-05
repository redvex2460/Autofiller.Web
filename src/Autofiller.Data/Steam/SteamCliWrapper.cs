using Autofiller.Data.Models;
using Autofiller.Data.Models.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Autofiller.Data.Steam
{
    public class SteamCliWrapper
    {
        #region Public Constructors

        public SteamCliWrapper(DataManager dataManager)
        {
            DataManager = dataManager;
            ScriptPath = Path.Combine(Directory.GetCurrentDirectory(), "steam", "steamcmd.sh");
            ScriptDirectory = Path.Combine(Directory.GetCurrentDirectory(), "steam");
            if (!File.Exists(ScriptPath))
                Initialise();
        }

        #endregion Public Constructors

        #region Public Properties

        public string CurrentOutput { get; set; }
        public DataManager DataManager { get; set; }
        public Process DownloadProcess { get; set; }
        public string ScriptDirectory { get; set; }
        public string ScriptPath { get; set; }

        #endregion Public Properties

        #region Private Properties

        private Task DownloadTask { get; set; }

        #endregion Private Properties

        #region Public Methods

        public dynamic CheckAuthorisedUsers()
        {
            dynamic result = new ExpandoObject();
            result.success = false;
            result.message = "";

            if (DataManager.AuthorisedUsers.Data.Count <= 0)
            {
                result.message += "No Authorised Steam accounts, please navigate to Settings and authorise an account.\n";
                result.message += "Thanks.\n";
            }
            var users = DataManager.AuthorisedUsers.Data;
            foreach (var account in users)
            {
                var steamCommand = SteamCommand.Init()
                                    .Login(account.UserName)
                                    .Execute();
                if (!steamCommand.Result)
                {
                    result.message += $"Could not authorise {account.UserName}, please reauthenticate in settings\n";
                    DataManager.AuthorisedUsers.Remove(account);
                }
            }

            return result;
        }

        public void GetApps(bool isForced = false)
        {
            string SteamAppListUrl = "https://api.steampowered.com/ISteamApps/GetAppList/v2";

            var lastUpdate = DataManager.Settings.LastUpdate;
            if (!isForced && lastUpdate + TimeSpan.FromDays(2) > DateTime.Now)
            {
                Console.WriteLine("No Update needed");
                return;
            }

            Console.WriteLine("Downloading App-List ....");
            var webClient = new WebClient();
            var response = JsonConvert.DeserializeObject<JObject>(webClient.DownloadString(SteamAppListUrl));
            var apps = response["applist"]["apps"].ToObject<List<App>>();
            DataManager.Apps.Data = apps;
            DataManager.Settings.LastUpdate = DateTime.Now;
            DataManager.Settings.SaveToDatabase();
            DataManager.Apps.SaveToDatabase();
            Console.WriteLine("App-List is now up to date.");
            return;
        }

        public bool Login(string account, string password, string guard = "")
        {
            if (DataManager.AuthorisedUsers.Data.Find(user => user.UserName.ToLower() == account.ToLower()) != null)
            {
                Console.WriteLine("That account is already authorized");
                return true;
            }
            var cmd = SteamCommand.Init().Login(account, password, guard).Execute();
            if (cmd.Result == true)
            {
                DataManager.AuthorisedUsers.Add(new AuthorisedUser() { UserName = account });
            }
            return cmd.Result;
        }

        public string StartDownload()
        {
            if (DownloadTask != null && DownloadTask.Status == TaskStatus.Running)
                return "Download is already in progress....";
            DownloadTask = Download();
            return "Download Started.";
        }

        #endregion Public Methods

        #region Internal Methods

        internal void Initialise()
        {
            var command = SteamCommand.Init().Install();
        }

        #endregion Internal Methods

        #region Private Methods

        private void DeleteDownloadedFiles()
        {
            CurrentOutput += "Deleting Downloaded files now...\n";
            if (Directory.Exists(DataManager.Settings.DownloadDirectory))
                Directory.Delete(DataManager.Settings.DownloadDirectory, true);
            if (!Directory.Exists(DataManager.Settings.DownloadDirectory))
                CurrentOutput += "Deleted Downloaded files.\n";
        }

        private Task Download()
        {
            CurrentOutput += CheckAuthorisedUsers().message;
            foreach (var app in DataManager.Queue.Data.Where(item => item.Status == DownloadStatus.Queued).ToList())
            {
                foreach (var user in DataManager.AuthorisedUsers.Data)
                {
                    CurrentOutput += $"Try to download {app.Name} via {user.UserName}\n";

                    var steamCommand = SteamCommand.Init()
                                       .Login(user.UserName)
                                       .SetPlatform(Enum.Parse<SteamPlatforms>(app.Platform))
                                       .SetDirectory(DataManager.Settings.DownloadDirectory)
                                       .Update(app.AppId)
                                       .Execute();

                    if (!steamCommand.Result)
                    {
                        CurrentOutput += "Download Failed\n";
                        continue;
                    }

                    CurrentOutput += "Download finished\n";
                    DataManager.Queue.Data.Find(item => item.AppId == app.AppId).Status = DownloadStatus.Completed;
                    DataManager.Queue.Data.Find(item => item.AppId == app.AppId).Update();
                    DeleteDownloadedFiles();
                }
            }
            return Task.CompletedTask;
        }

        #endregion Private Methods
    }
}