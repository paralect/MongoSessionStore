using MongoDB.Driver;

namespace MongoSessionStore
{
    public class MongoDB : MongoBase
    {
        /// <summary>
        /// 
        /// </summary>
        public MongoDB(MongoServer server)
            : base(server)
        {

        }

        public string DataBaseName = "AspSessionState";

        /// <summary>
        /// MongoDB server
        /// </summary>
        public override MongoDatabase Database
        {
            get { return Server.GetDatabase(DataBaseName); }
        }

        public MongoCollection GetCollection(string name)
        {
            return Database.GetCollection(name);
        }
    }

}