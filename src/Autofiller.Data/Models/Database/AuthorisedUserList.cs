using System;
using System.Collections.Generic;
using System.Text;

namespace Autofiller.Data.Models.Database
{
    public class AuthorisedUserList : IDatabaseTable<List<AuthorisedUser>>
    {
        public string Table => "Steam_Accounts";

        public void Add(AuthorisedUser user)
        {
            Data.Add(user);
            SaveToDatabase();
            Console.WriteLine($"Added Authoriseduser {user.UserName} to Database");
        }
        public void Remove(AuthorisedUser user)
        {
            Data.Remove(user);
            SaveToDatabase();
        }

        public List<AuthorisedUser> Data { get; set; } = new List<AuthorisedUser>();

        public void LoadFromDatabase()
        {
            var result = DatabaseConnector.GetRows($"SELECT * FROM {Table}");
            foreach (var entry in result)
            {
                Data.Add(new AuthorisedUser()
                {
                    UserName = (string)entry["UserName"]
                });
            }
            return;
        }

        public void SaveToDatabase()
        {

            DatabaseConnector.Command().OpenDatabase().SelectTable(Table).SaveTable(Data).Execute();
        }
    }
}
