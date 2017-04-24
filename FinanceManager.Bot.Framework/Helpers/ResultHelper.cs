using System.Collections.Generic;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;
using FinanceManager.Bot.Helpers.Enums;

namespace FinanceManager.Bot.Framework.Helpers
{
    public class ResultHelper
    {
        public HandlerServiceResult BuildOperationInvalidDateErrorResult()
        {
            return new HandlerServiceResult
            {
                Message = "Please, enter a valid date.",
                StatusCode = StatusCodeEnum.Bad
            };
        }

        public HandlerServiceResult BuildFinishedOperationConfiguringResult()
        {
            return new HandlerServiceResult
            {
                Message = "You successfully created operation.",
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