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
            Data = DatabaseCommand.Init(DatabaseConnector.SqliteConnection)
                .OpenDatabase()
                .GetObjects<App>($"SELECT * FROM {Table}")
                .Execute()
                .Result as List<App>;
            return;
        }

        public void SaveToDatabase()
        {
            DatabaseConnector.Command().OpenDatabase().SelectTable(Table).SaveTable(Data).Execute();
        }
    }
}
