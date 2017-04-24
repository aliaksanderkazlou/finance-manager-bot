using System.Threading.Tasks;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;
using Telegram.Bot;
using Message = FinanceManager.Bot.Helpers.Models.Message;

namespace FinanceManager.Bot.Framework.CommandHandlerServices
{
    public class HelpCommandHandlerService : ICommandHandlerService
    {
        private const string HelpText = "Here is a list of commands I can execute\n" +
                                          "/help - Find out what I can do\n" +
                                          "/category - Add, edit or delete categories";

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