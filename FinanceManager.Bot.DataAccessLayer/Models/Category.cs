using FinanceManager.Bot.Helpers.Enums;

namespace FinanceManager.Bot.DataAccessLayer.Models
{
    public class Category : BaseModel
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public long ExpenseForThisMonthInCents { get; set; }

        public long SupposedToSpentThisMonthInCents { get; set; }

        public long ExpenseInCents { get; set; }

        public long IncomeForThisMonthInCents { get; set; }

        public long IncomeInCents { get; set; }

        public string Currency { get; set; }

        public CategoryTypeEnum Type { get; set; }

        public bool Configured { get; set; }
    }
}