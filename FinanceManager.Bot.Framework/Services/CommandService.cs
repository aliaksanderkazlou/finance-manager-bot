using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.DataAccessLayer.Services.Logs;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.DataAccessLayer.Services.UserStatuses;
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
        private readonly StatsCommandHandlerService _statsCommandHandlerService;
        private readonly ILogDocumentService _logDocumentService;
        private readonly IUserDocumentService _userDocumentService;
        private readonly IUserStatusDocumentService _userStatusDocumentService;

        public CommandService(
            HelpCommandHandlerService helpCommandHandlerService,
            CategoryCommandHandlerService categoryCommandHandlerService,
            UnhandledMessageService unhandledMessageService,
            CancelCommandHandlerService cancelCommandHandlerService,
            OperationCommandHandlerService operationCommandHandlerService,
            StartCommandHandlerService startCommandHandlerService,
            StatsCommandHandlerService statsCommandHandlerService,
            IUserDocumentService userDocumentService,
            ILogDocumentService logDocumentService,
            IUserStatusDocumentService userStatusDocumentService)
        {
            _helpCommandHandlerService = helpCommandHandlerService;
            _categoryCommandHandlerService = categoryCommandHandlerService;
            _unhandledMessageService = unhandledMessageService;
            _userDocumentService = userDocumentService;
            _cancelCommandHandlerService = cancelCommandHandlerService;
            _operationCommandHandlerService = operationCommandHandlerService;
            _statsCommandHandlerService = statsCommandHandlerService;
            _startCommandHandlerService = startCommandHandlerService;
            _logDocumentService = logDocumentService;
            _userStatusDocumentService = userStatusDocumentService;
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
                {"/income", _operationCommandHandlerService.Handle },
                {"/expense", _operationCommandHandlerService.Handle },
                {"/stats", _statsCommandHandlerService.Handle }
            };
        }

        public async Task<List<HandlerServiceResult>> ExecuteCommand(string command, Message message)
        {
            List<HandlerServiceResult> result;
            var log = new Logs {Id = _logDocumentService.GenerateNewId()};

            try
            {
                var user = await _userDocumentService.GetByChatId(message.UserInfo.ChatId);

                if (user != null)
                {
                    log.Request = new LogRequest
                    {
                        ChatId = message.ChatInfo.Id,
                        Context = user.Context,
                        Message = message.Text,
                        UserId = user.Id
                    };

                    var userStatus = await _userStatusDocumentService.GetByUserId(user.Id);

                    userStatus.IsActiveLastThirtyDays = true;
                    userStatus.UpdatedAt = DateTime.UtcNow;

                    await _userStatusDocumentService.UpdateAsync(userStatus);
                }

                if (!command.Equals("/cancel") && user?.Context?.CurrentNode != null && user.Context.CurrentNode.Question != QuestionsEnum.None)
                {
                    if (user.Context.CurrentNode.Question.IsCategoryEnum())
                    {
                        result = await _categoryCommandHandlerService.HandleCategoryQuestion(message.Text, user);
                    }
                    else if (user.Context.CurrentNode.Question.IsOperationEnum())
                    {
                        result = await _operationCommandHandlerService.HandleOperationQuestion(message.Text, user);
                    }
                    else if (user.Context.CurrentNode.Question.IsStatsEnum())
                    {
                        result = await _statsCommandHandlerService.HandleStatsQuestion(message.Text, user);
                    }
                    else
                    {
                        result = await _unhandledMessageService.Handle(message);
                    }
                }
                else
                {
                    result = await _commandHandlerDictionary[command].Invoke(message);
                }
            }
            catch (KeyNotFoundException exception)
            {
                result = await _unhandledMessageService.Handle(message, exception);
            }
            catch (Exception exception)
            {
                result = await _unhandledMessageService.Handle(message, exception);
            }

            log.Responses = new List<LogResponse>();
            foreach (var serviceResult in result)
            {
                log.Responses.Add(new LogResponse
                {
                    Helper = serviceResult.Helper,
                    Message = serviceResult.Message
                });
            }

            await _logDocumentService.InsertAsync(log);

            return result;
        }
    }
}