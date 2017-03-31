using MongoDB.Bson.Serialization.Attributes;

namespace FinanceManager.Bot.DataAccessLayer.Models
{
    public class Chat
    {
        [BsonId]
        public string Id { get; set; }
    }
}