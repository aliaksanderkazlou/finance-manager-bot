using System.Collections.Generic;
using FinanceManager.Bot.Helpers.Enums;

namespace FinanceManager.Bot.DataAccessLayer.Models
{
    public class Category : BaseModel
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public long SpentThisMonthInCents { get; set; }

        public long SupposedToSpentThisMonthInCents { get; set; }

        public long SpentInCents { get; set; }

        public CategoryTypeEnum Type { get; set; }
    }
}