using FinanceManager.Bot.DataAccessLayer.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FinanceManager.Bot.DataAccessLayer
{
    public class MongoService
    {
        private readonly MongoClient _client;

        private readonly IMongoDatabase _database;

        public MongoService(string connectionString)
        {
            var mongoUrl = MongoUrl.Create(connectionString);
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(mongoUrl.DatabaseName);
        }

        public IMongoCollection<BsonDocument> Users => _database.GetCollection<BsonDocument>("users");
    }
}