using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceManager.Bot.Framework.Results;
using Message = FinanceManager.Bot.Helpers.Models.Message;

namespace FinanceManager.Bot.Framework
{
    public interface ICommandHandlerService
    {
        Task<List<HandlerServiceResult>> Handle(Message message);
    }
}