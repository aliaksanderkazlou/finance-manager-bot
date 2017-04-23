using FinanceManager.Bot.Helpers.Enums;

namespace FinanceManager.Bot.DataAccessLayer.Models
{
    public class Context
    {
        public QuestionsEnum LastQuestion { get; set; }

        public string OperationId { get; set; }

        public string CategoryId { get; set; }
    }
}