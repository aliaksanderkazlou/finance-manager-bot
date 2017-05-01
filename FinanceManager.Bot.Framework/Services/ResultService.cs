using System.Collections.Generic;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;

namespace FinanceManager.Bot.Framework.Services
{
    public class ResultService
    {
        public HandlerServiceResult BuildStatsNoOperationsOnDateRangeErrorResult()
        {
            return new HandlerServiceResult
            {
                Message = "There are no operations on this date range",
                StatusCode = StatusCodeEnum.Ok
            };
        }

        public HandlerServiceResult BuildCategoryInvalidCurrencyErrorResult()
        {
            return new HandlerServiceResult
            {
                Message = "You should select one of 3 options.",
                StatusCode = StatusCodeEnum.NeedKeyboard,
                Helper = new List<string>
                {
                    "EUR",
                    "USD",
                    "BYN"
                }
            };
        }

        public HandlerServiceResult BuildStatsWrongArgumentErrorResult()
        {
            return new HandlerServiceResult
            {
                Message = "Sorry, you should select one of 4 variants.",
                StatusCode = StatusCodeEnum.NeedKeyboard,
                Helper = new List<string>
                {
                    "All categories",
                    "Income only",
                    "Expense only",
                    "Custom category"
                }
            };
        }

        public HandlerServiceResult BuildCategoryInvalidSupposedToSpent()
        {
            return new HandlerServiceResult
            {
                Message = "Sorry, you can type only number greater than 0, or you can /cancel command.",
                StatusCode = StatusCodeEnum.Bad
            };
        }
        
        public HandlerServiceResult BuildCategoryInvalidTypeErrorResult()
        {
            return new HandlerServiceResult
            {
                Message = "Sorry, you can type only Income or Expense, or you can /cancel command",
                Helper = new List<string>
                {
                    "Income",
                    "Expense"
                },
                StatusCode = StatusCodeEnum.NeedKeyboard
            };
        }

        public HandlerServiceResult BuildNotUniqueCategoryNameErrorResult()
        {
            return new HandlerServiceResult
            {
                Message = "Sorry, name should be unique.",
                StatusCode = StatusCodeEnum.Bad
            };
        }

        public HandlerServiceResult BuildLongCategoryNameErrorResult()
        {
            return new HandlerServiceResult
            {
                Message = "Sorry, you cannot type more than 30 symbols",
                StatusCode = StatusCodeEnum.Bad
            };
        }

        public HandlerServiceResult BuildClearCategoryNameErrorResult()
        {
            return new HandlerServiceResult
            {
                Message = "Sorry, you should type something",
                StatusCode = StatusCodeEnum.Bad
            };
        }

        public HandlerServiceResult BuildCategoryActionsResult()
        {
            return new HandlerServiceResult
            {
                Message = "You always can create, edit or delete categories by /category command",
                StatusCode = StatusCodeEnum.Ok
            };
        }

        public HandlerServiceResult BuildYouShouldTypeOnlyYesOrNoErrorResult()
        {
            return new HandlerServiceResult
            {
                Message = "Sorry, you can type only Yes or No, or you can /cancel command.",
                Helper = new List<string>
                {
                    "Yes",
                    "No"
                },
                StatusCode = StatusCodeEnum.NeedKeyboard
            };
        }

        public HandlerServiceResult BuildCategoryNotFoundErrorResult()
        {
            return new HandlerServiceResult
            {
                Message = "Sorry, you don't have any category with this name.",
                StatusCode = StatusCodeEnum.Ok
            };
        }

        public HandlerServiceResult BuildCategoryDeletedResult()
        {
            return new HandlerServiceResult
            {
                Message = "Category was deleted successfully.",
                StatusCode = StatusCodeEnum.Ok
            };
        }

        public HandlerServiceResult BuildCategoryActionWrongAnswerErrorResult()
        {
            return new HandlerServiceResult
            {
                Helper = new List<string>
                {
                    "Add new category",
                    "Edit category",
                    "Delete category"
                },
                Message = "Sorry, you should chose one of three options, or you can /cancel command.",
                StatusCode = StatusCodeEnum.NeedKeyboard
            };
        }

        public HandlerServiceResult BuildOperationInvalidDateErrorResult()
        {
            return new HandlerServiceResult
            {
                Message = "Please, enter a valid date.",
                StatusCode = StatusCodeEnum.Bad
            };
        }

        public HandlerServiceResult BuildOperationCategoryNotFoundErrorResult()
        {
            return new HandlerServiceResult
            {
                Message = "Category with this name not found.",
                StatusCode = StatusCodeEnum.Bad
            };
        }

        public HandlerServiceResult BuildEmptyAnswerErrorResult()
        {
            return new HandlerServiceResult
            {
                Message = "You should type something.",
                StatusCode = StatusCodeEnum.Bad
            };
        }

        public HandlerServiceResult BuildFinishedConfiguringOperationResult()
        {
            return new HandlerServiceResult
            {
                Message = "You successfully configured your operation.",
                StatusCode = StatusCodeEnum.Ok
            };
        }

        public HandlerServiceResult BuildFinishedConfiguringCategoryResult()
        {
            return new HandlerServiceResult
            {
                Message = "You successfully configured your category.",
                StatusCode = StatusCodeEnum.Ok
            };
        }

        public HandlerServiceResult BuildOperationInvalidSumErrorResult()
        {
            return new HandlerServiceResult
            {
                Message = "Invalid sum. Please, try again.",
                StatusCode = StatusCodeEnum.Bad
            };
        }

        public HandlerServiceResult BuildOperationInvalidTypeErrorResult()
        {
            return new HandlerServiceResult
            {
                Message = "You should type + or -",
                StatusCode = StatusCodeEnum.Bad
            };
        }

        public HandlerServiceResult BuildCleanCategoryList()
        {
            return new HandlerServiceResult
            {
                Message = "Sorry, there are no categories.",
                StatusCode = StatusCodeEnum.Bad
            };
        }

        public HandlerServiceResult BuildOperationTypeCleanCategoryList()
        {
            return new HandlerServiceResult
            {
                Message = "Sorry, there are no categories with this type.",
                StatusCode = StatusCodeEnum.Bad
            };
        }

        public HandlerServiceResult BuildErrorResult()
        {
            return new HandlerServiceResult
            {
                Message = "Sorry, something went wrong. Please, try /cancel the command and start again.",
                StatusCode = StatusCodeEnum.Bad
            };
        }

        public HandlerServiceResult BuildOperationExceededAmountForThisMonth(float difference)
        {
            return new HandlerServiceResult
            {
                Message = string.Format("You have exceeded the expected amount for this month by {0}", difference),
                StatusCode = StatusCodeEnum.Ok
            };
        }
    }
}