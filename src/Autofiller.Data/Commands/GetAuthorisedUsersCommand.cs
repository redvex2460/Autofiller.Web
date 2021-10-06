using Autofiller.Data;
using Autofiller.Data.Steam;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Autofiller.Data.Commands
{
    internal class GetAuthorisedUsersCommand : ICommand<List<string>>
    {
        DataManager DataManager => DataManager.GetInstance();
        public List<string> Result { get; set; } = new List<string>();

        public ICommand<List<string>> Execute()
        {
            var users = DataManager.AuthorisedUsers.Data.ToList();
            foreach (var account in users)
            {
                var steamCommand = SteamCommand.Init()
                                    .Login(account.UserName)
                                    .Execute();
                if (!steamCommand.Result)
                {
                    DataManager.AuthorisedUsers.Remove(account);
                }
                Result.Add(account.UserName);
            }
            return this;
        }
    }
}
