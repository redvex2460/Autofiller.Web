using System;
using System.Collections.Generic;
using System.Text;

namespace Autofiller.Data.Models.Database
{
    public class QueuedApp : IDatabaseObject
    {
        public QueuedApp()
        {

        }
        public QueuedApp(string name, string platform, DateTime queuedTime, long appId, string status)
        {
            Name = name;
            Platform = platform;
            QueuedTime = queuedTime;
            AppId = appId;
            Status = status;
        }

        public string Table => "Steam_Queue";

        public long AppId { get; set; }
        public string Name { get; set; }
        public string Platform { get; set; }
        public DateTime QueuedTime { get; set; }
        public string Status{ get; set; }

        public void Update(string value)
        {
            throw new NotImplementedException();
        }

        public void Dequeue()
        {
            throw new NotImplementedException();
        }
    }
}
