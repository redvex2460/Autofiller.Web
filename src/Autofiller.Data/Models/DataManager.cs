using Autofiller.Data.Models.Database;
using Autofiller.Data.Steam;
using System.IO;

namespace Autofiller.Data.Models
{
    public class DataManager
    {
        #region Private Fields

        private static DataManager _instance;

        #endregion Private Fields

        #region Public Constructors

        public DataManager()
        {
            SteamWrapper = new SteamCliWrapper(this);
            DatabaseConnector.Initialise(Path.Combine(Directory.GetCurrentDirectory(), "database.db"), this);
        }

        #endregion Public Constructors

        #region Public Properties

        public static DataManager Instance { get { if (_instance == null) _instance = new DataManager(); return _instance; } }
        public AppList Apps { get; set; } = new AppList();
        public AuthorisedUserList AuthorisedUsers { get; set; } = new AuthorisedUserList();
        public Queue Queue { get; set; } = new Queue();
        public Settings Settings { get; set; } = new Settings();
        public SteamCliWrapper SteamWrapper { get; set; }

        #endregion Public Properties
    }
}