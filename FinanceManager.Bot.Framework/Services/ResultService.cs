using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;

namespace FinanceManager.Bot.Framework.Services
{
    public class ResultService
    {
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

        public HandlerServiceResult BuildFinishedConfiguringResult()
        {
            return new HandlerServiceResult
            {
                Message = "Well done!",
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