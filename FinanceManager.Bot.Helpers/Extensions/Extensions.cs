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
                case QuestionsEnum.CategoryAction:
                case QuestionsEnum.CategorySupposedToSpentThisMonth:
                case QuestionsEnum.AddNewCategoryOrNot:
                case QuestionsEnum.CategoryName:
                case QuestionsEnum.ChooseCategoryToDelete:
                case QuestionsEnum.ChooseCategoryToEdit:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsOperationEnum(this QuestionsEnum questionsEnum)
        {
            switch (questionsEnum)
            {
                case QuestionsEnum.OperationCategory:
                case QuestionsEnum.OperationDate:
                case QuestionsEnum.OperationSum:
                case QuestionsEnum.OperationType:
                    return true;
                default:
                    return false;
            }
        }
    }
}