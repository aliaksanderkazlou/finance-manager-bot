namespace FinanceManager.Bot.DataAccessLayer.Models
{
    public class Category
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal SpentThisMonthInCents { get; set; }

        public decimal SupposedToSpentThisMonthInCents { get; set; }

        public decimal SpentInCents { get; set; }
    }
}