using MongoDB.Bson.Serialization.Attributes;

namespace FinanceManager.Bot.DataAccessLayer.Models
{
    public class Chat : BaseModel
    {
        public string UserName { get; set; }

        public long TelegramCharId { get; set; }

        public string Type { get; set; }
    }
}