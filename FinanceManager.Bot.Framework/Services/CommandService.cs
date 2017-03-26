using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;
using Telegram.Bot.Types;

namespace FinanceManager.Bot.Framework.Services
{
    public sealed class CommandService
    {
        private delegate Task<HandlerServiceResult> CommandHandlerDelegate(Message message);

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