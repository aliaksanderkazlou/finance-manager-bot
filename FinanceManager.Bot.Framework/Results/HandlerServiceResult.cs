using FinanceManager.Bot.Framework.Enums;
using Telegram.Bot.Types;

namespace FinanceManager.Bot.Framework.Results
{
    public sealed class HandlerServiceResult
    {
        public StatusCodeEnum StatusCode { get; set; }

        public string Message { get; set; }
    }
}