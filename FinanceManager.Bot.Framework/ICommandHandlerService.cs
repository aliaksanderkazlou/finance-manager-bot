using System.Threading.Tasks;
using FinanceManager.Bot.Framework.Results;
using Telegram.Bot.Types;

namespace FinanceManager.Bot.Framework
{
    public interface ICommandHandlerService
    { 
        Task<HandlerServiceResult> Handle(Message message);
    }
}