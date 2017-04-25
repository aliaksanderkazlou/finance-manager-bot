using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.Framework.Results;
using FinanceManager.Bot.Framework.CommandHandlerServices;
using FinanceManager.Bot.Helpers.Enums;
using FinanceManager.Bot.Helpers.Extensions;
using Message = FinanceManager.Bot.Helpers.Models.Message;

namespace FinanceManager.Bot.Framework.Services
{
    public sealed class CommandService
    {
        private delegate Task<List<HandlerServiceResult>> CommandHandlerDelegate(Message message);
        private Dictionary<string, CommandHandlerDelegate> _commandHandlerDictionary;
        private readonly HelpCommandHandlerService _helpCommandHandlerService;
        private readonly CategoryCommandHandlerService _categoryCommandHandlerService;
        private readonly CancelCommandHandlerService _cancelCommandHandlerService;
        private readonly UnhandledMessageService _unhandledMessageService;
        private readonly OperationCommandHandlerService _operationCommandHandlerService;
        private readonly StartCommandHandlerService _startCommandHandlerService;
        private readonly IUserDocumentService _userDocumentService;

        public CommandService(
            HelpCommandHandlerService helpCommandHandlerService,
            CategoryCommandHandlerService categoryCommandHandlerService,
            UnhandledMessageService unhandledMessageService,
            CancelCommandHandlerService cancelCommandHandlerService,
            OperationCommandHandlerService operationCommandHandlerService,
            StartCommandHandlerService startCommandHandlerService,
            IUserDocumentService userDocumentService)
        {
            _helpCommandHandlerService = helpCommandHandlerService;
            _categoryCommandHandlerService = categoryCommandHandlerService;
            _unhandledMessageService = unhandledMessageService;
            _userDocumentService = userDocumentService;
            _cancelCommandHandlerService = cancelCommandHandlerService;
            _operationCommandHandlerService = operationCommandHandlerService;
            _startCommandHandlerService = startCommandHandlerService;
            InitializeCommandHandlerDictionary();
        }

        private void InitializeCommandHandlerDictionary()
        {
            _commandHandlerDictionary = new Dictionary<string, CommandHandlerDelegate>
            {
                {"/help", _helpCommandHandlerService.Handle},
                {"/category", _categoryCommandHandlerService.Handle},
                {"/start", _startCommandHandlerService.Handle},
                {"/cancel", _cancelCommandHandlerService.Handle},
                {"/operation", _operationCommandHandlerService.Handle }
            };
        }

        public async Task<List<HandlerServiceResult>> ExecuteCommand(string command, Message message)
        {
            List<HandlerServiceResult> result;

            try
            {
                result = await _commandHandlerDictionary[command].Invoke(message);
            }
            catch (KeyNotFoundException)
            {
                var user = await _userDocumentService.GetByChatId(message.UserInfo.ChatId);

                try
                {
                    if (user.Context.LastQuestion != QuestionsEnum.None)
                    {
                        if (user.Context.LastQuestion.IsCategoryEnum())
                        {
                            result = await _categoryCommandHandlerService.HandleCategoryQuestion(message.Text, user);
                        }
                        else if (user.Context.LastQuestion.IsOperationEnum())
                        {
                            result = await _operationCommandHandlerService.HandleOperationQuestion(message.Text, user);
                        }
                        else
                        {
                            result = await _unhandledMessageService.Handle(message);
                        }
                    }
                    else
                    {
                        result = await _unhandledMessageService.Handle(message);
                    }
                }
                catch (KeyNotFoundException)
                {
                    result = await _unhandledMessageService.Handle(message);
                }
                catch (Exception)
                {
                    result = await _unhandledMessageService.Handle(message);
                }
            }

            return result;
        }
    }
}