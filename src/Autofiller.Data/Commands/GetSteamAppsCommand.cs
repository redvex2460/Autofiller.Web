using Autofiller.Data.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Autofiller.Data.Commands
{
    internal class GetSteamAppsCommand : ICommand<List<App>>
    {
        public GetSteamAppsCommand(bool isForced)
        {
            IsForced = isForced;
        }

        DataManager DataManager => DataManager.GetInstance();
        public bool IsForced { get; set; }
        public List<App> Result { get; set; }

        public ICommand<List<App>> Execute()
        {
            string SteamAppListUrl = "https://api.steampowered.com/ISteamApps/GetAppList/v2";

            var lastUpdate = DataManager.Settings.LastUpdate;
            if (!IsForced && lastUpdate + TimeSpan.FromDays(2) > DateTime.Now)
            {
                Console.WriteLine("No Update needed");
                return this;
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
            return this;
        }
    }
}
