using System.Collections.Generic;

namespace FinanceManager.Bot.DataAccessLayer.Models
{
    public class Category : BaseModel
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public decimal SpentThisMonthInCents { get; set; }

        public decimal SupposedToSpentThisMonthInCents { get; set; }

        public decimal SpentInCents { get; set; }

        public List<Operation> Operations { get; set; }
    }
}