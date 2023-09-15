using ISport.Odds.Models;
using MongoDB.Driver;

namespace ISport.Odds
{
    public class MongoContext
    {
        public IMongoDatabase Database { get; }

        public MongoContext(MongoDBSettings connectionSetting)
        {
            var client = new MongoClient(connectionSetting.ConnectionString);
            Database = client.GetDatabase(connectionSetting.DatabaseName);
        }
    }
}
