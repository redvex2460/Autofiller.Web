using System;
using System.Collections.Generic;
using System.Text;

namespace Autofiller.Data.Database
{
    public class QueuedApp : IDatabaseObject
    {
        public QueuedApp()
        {

        }
        public QueuedApp(string name, string platform, DateTime queuedTime, long appId)
        {
            Name = name;
            Platform = platform;
            QueuedTime = queuedTime;
            AppId = appId;
            Status = DownloadStatus.Queued;
        }

        public string Table => "Steam_Queue";

        public long AppId { get; set; }
        public string Name { get; set; }
        public string Platform { get; set; }
        public DateTime QueuedTime { get; set; }
        public DownloadStatus Status{ get; set; }

        public void Update()
        {
            DatabaseCommand.Init(DatabaseConnector.SqliteConnection)
                .OpenDatabase()
                .SelectTable(Table)
                .UpdateValue("Name", Name, $"WHERE AppId = {AppId}")
                .UpdateValue("Status", Status.ToString(), $"WHERE AppId = {AppId}")
                .Execute();
        }
    }
}
