using System;
using System.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace MongoSessionStore
{
    public class SessionStore
    {
        private string _applicationName;
        private string _connectionString;

        public SessionStore(string applicationName, string connectionString)
        {
            this._applicationName = applicationName;
            this._connectionString = connectionString;
        }

        private MongoDB _mongodb = null;
        public MongoDB MongoDb
        {
            get
            {
                if ( _mongodb == null)
                    _mongodb = new MongoDB(MongoServer.Create(_connectionString));
                return _mongodb;
            }
        }

        public void Insert(Session session)
        {
            MongoDb.GetCollection(_applicationName).Insert(session.ToBsonDocument());
        }

        public Session Get(string id, string applicationName)
        {
            Session session;
            
            var query = Query.And(
                Query.EQ("_id", id),
                Query.EQ("ApplicationName", applicationName)
            );

            var sessionDoc = MongoDb.GetCollection(_applicationName).FindOneAs<BsonDocument>(query);
            
            if (sessionDoc == null)
            {
                session = null;
            }
            else
            {
                session = new Session(sessionDoc);
            }



            return session;
        }

        public void UpdateSession(string id, int timeout, BsonBinaryData sessionItems, string applicationName, int sessionItemsCount, object lockId)
        {

           /* BsonDocument selector = new BsonDocument() { { "SessionId", id }, { "ApplicationName", applicationName }, { "LockId", lockId } };
            BsonDocument session = new BsonDocument() { { "$set", new Document() { }, {  } } } };
            using (var mongo = new MongoDB(connectionString))
            {
                mongo.Connect();
                mongo[applicationName]["sessions"].Update(session, selector, 0, false);
            }*/

            var query = Query.And(
                Query.EQ("_id", id),
                Query.EQ("ApplicationName", applicationName),
                Query.EQ("LockID", BsonValue.Create(lockId))
            );

            var update = Update.Set("Expires", DateTime.Now.AddMinutes((double) timeout))
                .Set("Timeout", timeout)
                .Set("Locked", false)
                .Set("SessionItems", sessionItems)
                .Set("SessionItemsCount", sessionItemsCount);

            MongoDb.GetCollection(_applicationName).Update(query, update);
        }

        public void UpdateSessionExpiration(string id, string applicationName, double timeout)
        {
            var query = Query.And(
                Query.EQ("_id", id),
                Query.EQ("ApplicationName", applicationName)
            );

            var update = Update.Set("Expires", DateTime.Now.AddMinutes(timeout));

            MongoDb.GetCollection(_applicationName).Update(query, update);
        }

        public void EvictSession(Session session)
        {
            var query = Query.And(
                Query.EQ("_id", session.SessionID),
                Query.EQ("ApplicationName", session.ApplicationName),
                Query.EQ("LockID", BsonValue.Create(session.LockID))
            );

            MongoDb.GetCollection(_applicationName).Remove(query);
        }

        public void EvictSession(string id, string applicationName, object lockId)
        {
            var query = Query.And(
                Query.EQ("_id", id),
                Query.EQ("ApplicationName", applicationName),
                Query.EQ("LockID", BsonValue.Create(lockId))
            );

            MongoDb.GetCollection(_applicationName).Remove(query);
        }

        public void EvictExpiredSession(string id, string applicationName)
        {
            var query = Query.And(
                Query.EQ("_id", id),
                Query.EQ("ApplicationName", applicationName),
                Query.LT("Expires", DateTime.Now)
            );

            MongoDb.GetCollection(_applicationName).Remove(query);
        }

        public void LockSession(Session session)
        {
            var query = Query.And(
                Query.EQ("_id", session.SessionID),
                Query.EQ("ApplicationName", session.ApplicationName)
            );

            var update = Update.Set("LockDate", DateTime.Now)
                               .Set("Locked", true)
                               .Set("LockID", BsonValue.Create(session.LockID))
                               .Set("Flags",0);
            MongoDb.GetCollection(_applicationName).Update(query, update);
        }

        public void ReleaseLock(string id, string applicationName, object lockId, double timeout)
        {
            var query = Query.And(
                Query.EQ("_id", id),
                Query.EQ("ApplicationName", applicationName),
                Query.EQ("LockID", BsonValue.Create(lockId))
            );

            var update = Update.Set("Expires", DateTime.Now.AddMinutes(timeout))
                               .Set("Locked", false);

            MongoDb.GetCollection(_applicationName).Update(query, update);

        }
    }
}
