using System;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace FinanceManager.Bot.Framework.Services
{
    public class CommandService
    {
        private delegate void CommandHandlerDelegate(Message message);

        private Dictionary<string, CommandHandlerDelegate> _commandHandlerDictionary;

        private readonly ICommandHandlerService _inlineCommandHandlerService;

        private readonly ICommandHandlerService _helpCommandHandlerService;

        public CommandService(
            ICommandHandlerService inlineCommandHandlerService,
            ICommandHandlerService helpCommandHandlerService)
        {
            _inlineCommandHandlerService = inlineCommandHandlerService;
            _helpCommandHandlerService = helpCommandHandlerService;
            InitializeCommandHandlerDictionary();
        }

        private void InitializeCommandHandlerDictionary()
        {
            _commandHandlerDictionary = new Dictionary<string, CommandHandlerDelegate>
            {
                {"/inline", _inlineCommandHandlerService.Handle},
                {"/help", _helpCommandHandlerService.Handle}
            };
        }

        public void ExecuteCommand(string command, Message message)
        {
            try
            {
                _commandHandlerDictionary[command].Invoke(message);
            }
            catch (KeyNotFoundException)
            {
                // TODO: add error handler
            }
        }
    }
}