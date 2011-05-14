using System;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoSessionStore
{
    public abstract class MongoBase
    {
        /// <summary>
        /// MongoDB Server 
        /// </summary>
        private readonly MongoServer _server;

        /// <summary>
        /// MongoDB Server
        /// </summary>
        public MongoServer Server
        {
            get { return _server; }
        }

        /// <summary>
        /// 
        /// </summary>
        public MongoBase(MongoServer server)
        {
            _server = server;
        }

        /// <summary>
        /// MongoDB server
        /// </summary>
        public abstract MongoDatabase Database { get; }

    }

}