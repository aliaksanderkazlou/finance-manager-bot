using System.Threading.Tasks;
using FinanceManager.Bot.Framework.Results;
using Telegram.Bot.Types;

namespace FinanceManager.Bot.Framework.CommandHandlerServices
{
    public class InlineCommandHandlerService : ICommandHandlerService
    {
        public InlineCommandHandlerService()
        {
            
        }

        public Task<HandlerServiceResult> Handle(Message message)
        {
            return (Task<HandlerServiceResult>) Task.CompletedTask;
        }
    }
}