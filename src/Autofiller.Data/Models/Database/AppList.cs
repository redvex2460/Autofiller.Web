using System;
using System.Collections.Generic;
using System.Text;

namespace Autofiller.Data.Models.Database
{
    public class AppList : IDatabaseTable<List<App>>
    {
        public string Table => "Steam_Apps";

        public List<App> Data { get; set; } = new List<App>();

        public void LoadFromDatabase()
        {
            var result = DatabaseConnector.GetRows($"SELECT * FROM {Table}");
            foreach(var entry in result)
            {
                var appId = (long)entry["AppId"];
                var appName = (string)entry["Name"];
                var app = new App(appId, appName);
                Data.Add(app);
            }
            return;
        }

        public void SaveToDatabase()
        {
            DatabaseConnector.ExecuteQuery($"DELETE FROM {Table}");
            DatabaseConnector.SqliteConnection.Open();
            using (var transaction = DatabaseConnector.SqliteConnection.BeginTransaction())
            {
                var command = DatabaseConnector.SqliteConnection.CreateCommand();
                command.CommandText =
                @$"
                    INSERT INTO {Table}
                    VALUES ($appid, $name)
                ";

                var appIdParameter = command.CreateParameter();
                appIdParameter.ParameterName = "$appid";
                command.Parameters.Add(appIdParameter);

                var nameParameter = command.CreateParameter();
                nameParameter.ParameterName = "name";
                command.Parameters.Add(nameParameter);

                foreach(var app in Data)
                {
                    appIdParameter.Value = app.AppId;
                    nameParameter.Value = app.Name;
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            DatabaseConnector.SqliteConnection.Close();
        }
    }
}
