using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace FinanceManager.Bot.DataAccessLayer.Models
{
    public class User
    {
        [BsonId]
        public string Id { get; set; }

        public List<Category> Categories { get; set; }
    }
}