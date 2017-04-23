using System;
using FinanceManager.Bot.Helpers.Enums;

namespace FinanceManager.Bot.Helpers.Extensions
{
    public static class Extensions
    {
        public static bool IsCategoryEnum(this QuestionsEnum questionsEnum)
        {
            switch (questionsEnum)
            {
                case QuestionsEnum.CategoryType:
                case QuestionsEnum.CategoryCurrency:
                case QuestionsEnum.CategoryOperation:
                case QuestionsEnum.CategorySupposedToSpentThisMonth:
                    return true;
                case QuestionsEnum.None:
                    return false;
                default:
                    return false;
            }
        }
    }
}