using Autofiller.Data.Commands;
using Autofiller.Data.Database;
using Autofiller.Data.Steam;
using Autofiller.Data.Steam.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Autofiller.Data
{
    public class DataManager
    {
        #region Private Fields

        private static DataManager _instance;

        #endregion Private Fields

        #region Public Constructors

        public DataManager()
        {
            InitialiseSteam();
            DatabaseConnector.Initialise(Path.Combine(Directory.GetCurrentDirectory(), "database.db"), this);
        }

        public bool Login(string username, string password, string guard)
        {
            return new AuthoriseUserCommand(username, password, guard).Execute().Result;
        }

        public void StartDownload()
        {
            DownloadManager = new DownloadManager(Queue);
            DownloadManager.StartDownload();
        }

        #endregion Public Constructors

        #region Public Properties

        public AppList Apps { get; set; } = new AppList();
        public AuthorisedUserList AuthorisedUsers { get; set; } = new AuthorisedUserList();
        public Queue Queue { get; set; } = new Queue();
        public Settings Settings { get; set; } = new Settings();
        public DownloadManager DownloadManager { get; set; }
        public bool IsDownloading => DownloadManager != null && DownloadManager.DownloadThread != null;
        public string ScriptDirectory = Path.Combine(Directory.GetCurrentDirectory(), "steam");
        public string ScriptPath = Path.Combine(Directory.GetCurrentDirectory(), "steam", "steamcmd.sh");

        public static DataManager GetInstance()
        {
            if (_instance == null)
                _instance = new DataManager();
            return _instance;
        }

        public string StopDownload()
        {
            DownloadManager.StopDownload();
            DownloadManager = null;
            return "Downloads successfully stopped.";
        }

        internal void InitialiseSteam()
        {
            SteamCommand.Init().Install();
        }

        #endregion Public Properties
    }
}