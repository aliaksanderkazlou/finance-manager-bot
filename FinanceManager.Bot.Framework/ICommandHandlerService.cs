using System.Threading.Tasks;
using FinanceManager.Bot.Framework.Results;
using Message = FinanceManager.Bot.Helpers.Models.Message;

namespace FinanceManager.Bot.Framework
{
    public interface ICommandHandlerService
    { 
        Task<HandlerServiceResult> Handle(Message message);
    }
}