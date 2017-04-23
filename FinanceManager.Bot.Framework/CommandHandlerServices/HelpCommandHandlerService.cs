using System.Threading.Tasks;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FinanceManager.Bot.Framework.CommandHandlerServices
{
    public class HelpCommandHandlerService : ICommandHandlerService
    {
        private const string HelpText = "Here is a list of commands I can execute\n" +
                                          "/inline - inline\n" +
                                          "/help - Find out what I can do\n" +
                                          "/categories - Add, edit or delete categories";

        public HelpCommandHandlerService()
        {
        }

        public async Task<HandlerServiceResult> Handle(Message message)
        {
            return new HandlerServiceResult
            {
                Message = HelpText,
                StatusCode = StatusCodeEnum.Ok
            };
        }
    }
}