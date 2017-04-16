using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.Helpers.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FinanceManager.Bot.DataAccessLayer
{
    public class MongoService
    {
        private readonly MongoClient _client;

        private readonly IMongoDatabase _database;

        public MongoService(AppSettings settings)
        {
            var mongoUrl = MongoUrl.Create(settings.DatabaseConnectionString);
            _client = new MongoClient(settings.DatabaseConnectionString);
            _database = _client.GetDatabase(mongoUrl.DatabaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    }
}