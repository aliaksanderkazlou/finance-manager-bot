using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.Helpers.Models;
using MongoDB.Driver;

namespace FinanceManager.Bot.DataAccessLayer
{
    public class MongoService
    {
        private readonly IMongoDatabase _database;

        public MongoService(AppSettings settings)
        {
            var mongoUrl = MongoUrl.Create(settings.DatabaseConnectionString);
            var client = new MongoClient(settings.DatabaseConnectionString);
            _database = client.GetDatabase(mongoUrl.DatabaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("users");

        public IMongoCollection<UnhandledMessage> UnhandledMessages => _database.GetCollection<UnhandledMessage>(
            "unhandled_messages");

        public IMongoCollection<Category> Categories => _database.GetCollection<Category>("categories");
    }
}