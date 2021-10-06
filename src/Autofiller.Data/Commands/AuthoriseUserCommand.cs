using Autofiller.Data.Database;
using Autofiller.Data.Steam;
using System;

namespace Autofiller.Data.Commands
{
    internal class AuthoriseUserCommand : ICommand<bool>
    {
        #region Public Constructors

        public AuthoriseUserCommand(string username, string password, string guardcode)
        {
            Username = username;
            Password = password;
            Guardcode = guardcode;
        }

        #endregion Public Constructors

        #region Public Properties

        public string Guardcode { get; set; }
        public bool IsAuthorised { get; private set; }
        public string Password { get; set; }
        public bool Result { get; set; }
        public string Username { get; set; }

        #endregion Public Properties

        #region Private Properties

        private DataManager DataManager => DataManager.GetInstance();

        #endregion Private Properties

        #region Public Methods

        public ICommand<bool> Execute()
        {
            Result = false;
            if (DataManager.AuthorisedUsers.Data.Find(user => user.UserName.ToLower() == Username.ToLower()) != null)
            {
                Console.WriteLine("That account is already authorized");
                Result = true;
                return this;
            }
            var cmd = SteamCommand.Init().Login(Username, Password, Guardcode).Execute();
            if (cmd.Result == true)
            {
                DataManager.AuthorisedUsers.Add(new AuthorisedUser() { UserName = Username });
                Result = true;
            }
            return this;
        }

        #endregion Public Methods
    }
}