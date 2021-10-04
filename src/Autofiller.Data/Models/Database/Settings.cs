using System;
using System.Collections.Generic;
using System.Text;

namespace Autofiller.Data.Models.Database
{
    public class Settings : IDatabaseTable
    {
        public string Table => "App_Settings";
        public DateTime LastUpdate { get; set; } = new DateTime();
        public string DownloadDirectory { get; set; } = "/tmp/autofill";

        public void LoadFromDatabase()
        {
            var result = DatabaseConnector.GetRows($"SELECT * FROM {Table}");
            foreach (var entry in result)
            {
                LastUpdate = DateTime.Parse(entry["LastUpdate"].ToString());
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
                    VALUES ($LastUpdate, $DownloadDirectory)

                ";

                var lastUpdateParameter = command.CreateParameter();
                lastUpdateParameter.ParameterName = "LastUpdate";
                lastUpdateParameter.Value = LastUpdate;
                command.Parameters.Add(lastUpdateParameter);

                var downloadDirectoryParameter = command.CreateParameter();
                downloadDirectoryParameter.ParameterName = "DownloadDirectory";
                downloadDirectoryParameter.Value = DownloadDirectory;
                command.Parameters.Add(downloadDirectoryParameter);
                command.ExecuteNonQuery();
                transaction.Commit();
            }
            DatabaseConnector.SqliteConnection.Close();
        }
    }
}
