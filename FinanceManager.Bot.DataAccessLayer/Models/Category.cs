namespace FinanceManager.Bot.DataAccessLayer.Models
{
    public class Category : BaseModel
    {
        public string Name { get; set; }

        public decimal SpentThisMonthInCents { get; set; }

        public decimal SupposedToSpentThisMonthInCents { get; set; }

        public decimal SpentInCents { get; set; }
    }
}