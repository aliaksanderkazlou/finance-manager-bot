using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;
using Telegram.Bot.Types;
using FinanceManager.Bot.Framework.CommandHandlerServices;
using FinanceManager.Bot.Helpers.Enums;
using FinanceManager.Bot.Helpers.Extensions;
using User = FinanceManager.Bot.DataAccessLayer.Models.User;

namespace FinanceManager.Bot.Framework.Services
{
    public sealed class CommandService
    {
        private delegate Task<HandlerServiceResult> CommandHandlerDelegate(Message message);
        //private delegate Task<HandlerServiceResult> QuestionHandlerDelegate(string answer, User user);
        private Dictionary<string, CommandHandlerDelegate> _commandHandlerDictionary;
        //private Dictionary<QuestionsEnum, QuestionHandlerDelegate> _questionHandlerDictionary;
        private readonly InlineCommandHandlerService _inlineCommandHandlerService;
        private readonly HelpCommandHandlerService _helpCommandHandlerService;
        private readonly CategoryCommandHandlerService _categoryCommandHandlerService;
        private readonly UnhandledMessageService _unhandledMessageService;
        private readonly IUserDocumentService _userDocumentService;

        public CommandService(
            InlineCommandHandlerService inlineCommandHandlerService,
            HelpCommandHandlerService helpCommandHandlerService,
            CategoryCommandHandlerService categoryCommandHandlerService,
            UnhandledMessageService unhandledMessageService,
            IUserDocumentService userDocumentService)
        {
            _inlineCommandHandlerService = inlineCommandHandlerService;
            _helpCommandHandlerService = helpCommandHandlerService;
            _categoryCommandHandlerService = categoryCommandHandlerService;
            _unhandledMessageService = unhandledMessageService;
            _userDocumentService = userDocumentService;
            InitializeCommandHandlerDictionary();
            //InitializeQuestionHandlerDictionary();
        }

        private void InitializeCommandHandlerDictionary()
        {
            _commandHandlerDictionary = new Dictionary<string, CommandHandlerDelegate>
            {
                {"/inline", _inlineCommandHandlerService.Handle},
                {"/help", _helpCommandHandlerService.Handle},
                {"/category", _categoryCommandHandlerService.Handle},
                {"/start", _helpCommandHandlerService.Handle}
                // TODO: cancel
            };
        }

        //private void InitializeQuestionHandlerDictionary()
        //{
        //    _questionHandlerDictionary = new Dictionary<QuestionsEnum, QuestionHandlerDelegate>
        //    {
        //        {QuestionsEnum.CategoryType, _categoryCommandHandlerService.ConfigureCategoryType },
        //        {QuestionsEnum.CategoryCurrency, _categoryCommandHandlerService.ConfigureCategoryCurrency },
        //        {QuestionsEnum.CategoryOperation, _categoryCommandHandlerService.ConfigureCategoryOperation },
        //        {QuestionsEnum.CategorySupposedToSpentThisMonth,  _categoryCommandHandlerService.ConfigureCategorySupposedToSpentThisMonth}
        //    };
        //}

        public async Task<HandlerServiceResult> ExecuteCommand(string command, Message message)
        {
            HandlerServiceResult result;

            try
            {
                result = await _commandHandlerDictionary[command].Invoke(message);
            }
            catch (KeyNotFoundException)
            {
                var user = await _userDocumentService.GetByChatId(message.Chat.Id);

                try
                {
                    if (user.Context.LastQuestion != QuestionsEnum.None)
                    {
                        if (user.Context.LastQuestion.IsCategoryEnum())
                        {
                            result = await _categoryCommandHandlerService.HandleCategoryQuestion(message.Text, user);
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
            }

            return result;
        }
    }
}