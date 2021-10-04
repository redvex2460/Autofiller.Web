using System;
using System.Collections.Generic;
using System.Text;

namespace Autofiller.Data.Models.Database
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
                    Status = (string)entry["Status"]
                });
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
                    VALUES ($appid, $name, $platform, $queuedtime, $status)

                ";

                var appIdParameter = command.CreateParameter();
                appIdParameter.ParameterName = "appid";
                command.Parameters.Add(appIdParameter);

                var nameParameter = command.CreateParameter();
                nameParameter.ParameterName = "name";
                command.Parameters.Add(nameParameter);

                var platformParameter = command.CreateParameter();
                platformParameter.ParameterName = "platform";
                command.Parameters.Add(platformParameter);

                var queuedTimeParameter = command.CreateParameter();
                queuedTimeParameter.ParameterName = "queuedtime";
                command.Parameters.Add(queuedTimeParameter);

                var statusParameter = command.CreateParameter();
                statusParameter.ParameterName = "status";
                command.Parameters.Add(statusParameter);

                foreach (var app in Data)
                {
                    appIdParameter.Value = app.AppId;
                    nameParameter.Value = app.Name;
                    platformParameter.Value = app.Platform;
                    queuedTimeParameter.Value = app.QueuedTime;
                    statusParameter.Value = app.Status;
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            DatabaseConnector.SqliteConnection.Close();
        }
    }
}
