using Telegram.Bot.Types;

namespace FinanceManager.Bot.Framework
{
    public interface ICommandHandlerService
    { 
        void Handle(Message message);
    }
}