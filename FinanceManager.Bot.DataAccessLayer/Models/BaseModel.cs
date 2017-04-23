using MongoDB.Bson.Serialization.Attributes;

namespace FinanceManager.Bot.DataAccessLayer.Models
{
    public abstract class BaseModel
    {
        [BsonId]
        public string Id { get; set; }
    }
}
