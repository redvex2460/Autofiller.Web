using System;
using System.Collections.Generic;
using System.Text;

namespace Autofiller.Data.Database
{
    public class Queue : IDatabaseTable<List<QueuedApp>>
    {
        public string Table => "Steam_Queue";

        public void Add(QueuedApp app)
        {
            Data.Add(app);
            SaveToDatabase();
            Console.WriteLine($"Added Steam App {app.Name} with ID {app.AppId} for {app.Platform} to download queue.");
        }
        public void Remove(QueuedApp app)
        {
            Data.Remove(app);
            SaveToDatabase();
        }

        public void Remove(Predicate<QueuedApp> match)
        {
            var app = Data.Find(match);
            if (app == null)
                return;
            Data.Remove(app);
            SaveToDatabase();
        }

        public List<QueuedApp> Data { get; set; } = new List<QueuedApp>();

        public void LoadFromDatabase()
        {
            var result = DatabaseConnector.GetRows($"SELECT * FROM {Table}");
            foreach (var entry in result)
            {
                Data.Add(new QueuedApp()
                {
                    AppId = long.Parse(entry["AppId"].ToString()),
                    Name = (string)entry["Name"],
                    Platform = (string)entry["Platform"],
                    QueuedTime = DateTime.Parse((string)entry["QueuedTime"]),
                    Status = Enum.Parse<DownloadStatus>((string)entry["Status"])
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
