using System.Collections.Generic;

namespace FinanceManager.Bot.Framework.Services
{
    public class CommandService
    {
        private delegate void CommandHandlerDelegate(string body);

        private Dictionary<string, CommandHandlerDelegate> _commandHandlerDictionary;

        private readonly ICommandHandlerService _inlineCommandHandlerService;

        public CommandService(ICommandHandlerService inlineCommandHandlerService)
        {
            _inlineCommandHandlerService = inlineCommandHandlerService;
            InitializeCommandHandlerDictionary();
        }

        private void InitializeCommandHandlerDictionary()
        {
            _commandHandlerDictionary = new Dictionary<string, CommandHandlerDelegate>
            {
                {"/inline", _inlineCommandHandlerService.Handle}
            };
        }

        public void ExecuteCommand(string command, string body)
        {
            _commandHandlerDictionary[command].Invoke(body);
        }
    }
}