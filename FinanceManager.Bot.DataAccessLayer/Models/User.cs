using System.Collections.Generic;

namespace FinanceManager.Bot.DataAccessLayer.Models
{
    public class User
    {
        public string Id { get; set; }

        public List<Category> Categories { get; set; }
    }
}