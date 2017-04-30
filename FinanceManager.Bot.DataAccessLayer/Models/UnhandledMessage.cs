using System;
using MongoDB.Bson.Serialization.Attributes;

namespace FinanceManager.Bot.DataAccessLayer.Models
{
    public class UnhandledMessage : BaseModel
    {
        public string Text { get; set; }

        public long ChatId { get; set; }

        public DateTime Created { get; set; }

        public Exception Exception { get; set; }
    }
}