using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;
using Telegram.Bot.Types;
using FinanceManager.Bot.Framework.CommandHandlerServices;

namespace FinanceManager.Bot.Framework.Services
{
    public sealed class CommandService
    {
        private delegate Task<HandlerServiceResult> CommandHandlerDelegate(Message message);

        private Dictionary<string, CommandHandlerDelegate> _commandHandlerDictionary;

        private readonly InlineCommandHandlerService _inlineCommandHandlerService;

        private readonly HelpCommandHandlerService _helpCommandHandlerService;

        private readonly CategoryCommandHandlerService _categoryCommandHandlerService;

        public CommandService(
            InlineCommandHandlerService inlineCommandHandlerService,
            HelpCommandHandlerService helpCommandHandlerService,
            CategoryCommandHandlerService categoryCommandHandlerService)
        {
            _inlineCommandHandlerService = inlineCommandHandlerService;
            _helpCommandHandlerService = helpCommandHandlerService;
            _categoryCommandHandlerService = categoryCommandHandlerService;
            InitializeCommandHandlerDictionary();
        }

        private void InitializeCommandHandlerDictionary()
        {
            _commandHandlerDictionary = new Dictionary<string, CommandHandlerDelegate>
            {
                {"/inline", _inlineCommandHandlerService.Handle},
                {"/help", _helpCommandHandlerService.Handle},
                {"/category", _categoryCommandHandlerService.Handle}
            };
        }

        public async Task<HandlerServiceResult> ExecuteCommand(string command, Message message)
        {
            try
            {
                return await _commandHandlerDictionary[command].Invoke(message);
            }
            catch (KeyNotFoundException)
            {
                // TODO: add error handler
                
            }

            return new HandlerServiceResult
            {
                Message = "",
                StatusCode = StatusCodeEnum.Bad
            };
        }
    }
}