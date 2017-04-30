using System.Linq;
using FinanceManager.Bot.Helpers.Enums;
using FinanceManager.Bot.Helpers.Structures;

namespace FinanceManager.Bot.Framework.Helpers
{
    public static class QuestionTreeHelper
    {
        public static QuestionTree FindChildByQuestion(this QuestionTree tree, QuestionsEnum question)
        {
            return tree.Children.FirstOrDefault(child => child.Question == QuestionsEnum.CategoryName);
        }
    }
}