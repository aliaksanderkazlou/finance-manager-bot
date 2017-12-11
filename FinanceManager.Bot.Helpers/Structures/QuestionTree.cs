using System.Collections.Generic;
using FinanceManager.Bot.Helpers.Enums;

namespace FinanceManager.Bot.Helpers.Structures
{
    public class QuestionTree
    {
        public List<QuestionTree> Children { get; set; }

        public QuestionsEnum Question { get; set; }
    }
}