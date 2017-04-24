﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.DataAccessLayer.Services.Categories;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;
using FinanceManager.Bot.Helpers.Enums;
using FinanceManager.Bot.Helpers.Models;

namespace FinanceManager.Bot.Framework.CommandHandlerServices
{
    public class StartCommandHandlerService : ICommandHandlerService
    {
        private readonly IUserDocumentService _userDocumentService;
        private readonly ICategoryDocumentService _categoryDocumentService;
        private const string HelpText = "Here is a list of commands I can execute\n" +
                                        "/help - Find out what I can do\n" +
                                        "/category - Add, edit or delete categories";

        public StartCommandHandlerService(
            IUserDocumentService userDocumentService,
            ICategoryDocumentService categoryDocumentService)
        {
            _userDocumentService = userDocumentService;
            _categoryDocumentService = categoryDocumentService;
        }

        public async Task<HandlerServiceResult> Handle(Message message)
        {
            var user = await _userDocumentService.GetByChatId(message.UserInfo.ChatId);

            if (user == null)
            {
                user = new User
                {
                    Id = _userDocumentService.GenerateNewId(),
                    ChatId = message.UserInfo.ChatId,
                    FirstName = message.UserInfo.FirstName,
                    LastName = message.UserInfo.LastName
                };

                var defaultIncomeCategory = new Category
                {
                    Id = _categoryDocumentService.GenerateNewId(),
                    Name = "Default Income Category",
                    Operations = new List<Operation>(),
                    SpentInCents = 0,
                    SpentThisMonthInCents = 0,
                    SupposedToSpentThisMonthInCents = 0,
                    Type = CategoryTypeEnum.Income
                };

                var defaultExpenseCategory = new Category
                {
                    Id = _categoryDocumentService.GenerateNewId(),
                    Name = "Default Expense Category",
                    Operations = new List<Operation>(),
                    SpentInCents = 0,
                    SpentThisMonthInCents = 0,
                    SupposedToSpentThisMonthInCents = 0,
                    Type = CategoryTypeEnum.Expense
                };

                await _userDocumentService.InsertAsync(user);
                await _categoryDocumentService.InsertAsync(defaultExpenseCategory);
                await _categoryDocumentService.InsertAsync(defaultIncomeCategory);
            }

            return new HandlerServiceResult
            {
                Message = HelpText,
                StatusCode = StatusCodeEnum.Ok
            };
        }
    }
}