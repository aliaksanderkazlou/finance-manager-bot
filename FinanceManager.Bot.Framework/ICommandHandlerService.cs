namespace FinanceManager.Bot.Framework
{
    public interface ICommandHandlerService
    {
        string Command { get; set; }

        void Handle(string body);
    }
}