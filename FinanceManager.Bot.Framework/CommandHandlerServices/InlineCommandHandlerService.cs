namespace FinanceManager.Bot.Framework.CommandHandlerServices
{
    public class InlineCommandHandlerService : ICommandHandlerService
    {
        public InlineCommandHandlerService()
        {
            
        }

        public string Command { get; set; }

        public void Handle(string body)
        {
            throw new System.NotImplementedException();
        }
    }
}