using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace FinanceManager.Bot.Framework
{
    public interface ICommandHandlerService
    { 
        Task Handle(Message message);
    }
}