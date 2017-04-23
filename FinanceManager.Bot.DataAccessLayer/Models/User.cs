using System.Collections.Generic;
using FinanceManager.Bot.Helpers.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace FinanceManager.Bot.DataAccessLayer.Models
{
    public class User : BaseModel
    {
        public long ChatId { get; set; }
        
        public Context Context { get; set; }
    }
}