using System;
using System.Configuration;
using System.Web.SessionState;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace MongoSessionStore
{
    [BsonIgnoreExtraElements]
    public class Session
    {
        [BsonId]
        public string SessionID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public string ApplicationName { get; set; }
        public DateTime LockDate { get; set; }
        public object LockID { get; set; }
        public int Timeout { get; set; }
        public bool Locked { get; set; }
        public BsonBinaryData SessionItems { get; set; }
        public int SessionItemsCount { get; set; }
        public int Flags { get; set; }

        public Session() { }

        public Session(string id, string applicationName, int timeout, BsonBinaryData sessionItems, int sessionItemsCount, SessionStateActions actionFlags)
        {
            SessionID = id;
            ApplicationName = applicationName;
            LockDate = DateTime.Now;
            LockID = 0;
            Timeout = timeout;
            Locked = false;
            SessionItems = sessionItems;
            SessionItemsCount = sessionItemsCount;
            Flags = (int)actionFlags;
            Created = DateTime.Now;
            Expires = DateTime.Now.AddMinutes((Double)this.Timeout);         
        }

        public Session(BsonDocument document)
        {
            if (document != null)
            {
                SessionID = (string) document["_id"];
                ApplicationName = (string) document["ApplicationName"];
                LockDate = (DateTime) document["LockDate"];
                LockDate = this.LockDate.ToLocalTime();
                LockID = (int) document["LockID"];
                Timeout = (int) document["Timeout"];
                Locked = (bool) document["Locked"];
                SessionItems = (BsonBinaryData) document["SessionItems"];
                SessionItemsCount = (int) document["SessionItemsCount"];
                Flags = (int) document["Flags"];
                Created = (DateTime) document["Created"];
                Created = this.Created.ToLocalTime();
                Expires = (DateTime) document["Expires"];
                Expires = this.Expires.ToLocalTime();
            }
        }
    }
}
