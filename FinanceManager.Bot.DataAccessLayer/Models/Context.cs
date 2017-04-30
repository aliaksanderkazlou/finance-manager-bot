using System.Collections.Generic;
using FinanceManager.Bot.Helpers.Enums;
using FinanceManager.Bot.Helpers.Structures;

namespace FinanceManager.Bot.DataAccessLayer.Models
{
    public class Context
    {
        public QuestionTree CurrentNode { get; set; }

        public string OperationId { get; set; }

        public string CategoryId { get; set; }

        public CategoryTypeEnum CategoryType { get; set; }
    }
}