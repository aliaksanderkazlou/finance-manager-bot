using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.DataAccessLayer.Services.Categories;
using FinanceManager.Bot.DataAccessLayer.Services.Chats;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.DataAccessLayer.Services.UserStatuses;
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
        private readonly IChatDocumentService _chatDocumentService;
        private readonly IUserStatusDocumentService _userStatusDocumentService;

        private const string HelpText = "/category - Add, edit or delete categories\n" +
                                        "/income - Add an income operation\n" +
                                        "/expense - Add an expense operation\n" +
                                        "/stats - Get statistics\n" +
                                        "/help - Find out what I can do\n" +
                                        "/cancel - Cancel the current command";

        public StartCommandHandlerService(
            IUserDocumentService userDocumentService,
            ICategoryDocumentService categoryDocumentService,
            IChatDocumentService chatDocumentService,
            IUserStatusDocumentService userStatusDocumentService)
        {
            _userDocumentService = userDocumentService;
            _categoryDocumentService = categoryDocumentService;
            _chatDocumentService = chatDocumentService;
            _userStatusDocumentService = userStatusDocumentService;
        }

        public async Task<List<HandlerServiceResult>> Handle(Message message)
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

                var chat = new Chat
                {
                    Id = _chatDocumentService.GenerateNewId(),
                    TelegramCharId = message.ChatInfo.Id,
                    Type = message.ChatInfo.Type,
                    UserName = message.ChatInfo.UserName
                };

                var defaultIncomeCategory = new Category
                {
                    Id = _categoryDocumentService.GenerateNewId(),
                    UserId = user.Id,
                    Name = "Default Income Category",
                    ExpenseInCents = 0,
                    ExpenseForThisMonthInCents = 0,
                    SupposedToSpentThisMonthInCents = 0,
                    Type = CategoryTypeEnum.Income,
                    Configured = true,
                    Currency = "BYN"
                };

                var defaultExpenseCategory = new Category
                {
                    Id = _categoryDocumentService.GenerateNewId(),
                    UserId = user.Id,
                    Name = "Default Expense Category",
                    ExpenseInCents = 0,
                    ExpenseForThisMonthInCents = 0,
                    SupposedToSpentThisMonthInCents = 0,
                    Type = CategoryTypeEnum.Expense,
                    Configured = true,
                    Currency = "BYN"
                };

                var userStatus = new UserStatus
                {
                    Id = _userStatusDocumentService.GenerateNewId(),
                    IsActiveLastThirtyDays = true,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = user.Id
                };


                await _userStatusDocumentService.InsertAsync(userStatus);
                await _chatDocumentService.InsertAsync(chat);
                await _userDocumentService.InsertAsync(user);
                await _categoryDocumentService.InsertAsync(defaultExpenseCategory);
                await _categoryDocumentService.InsertAsync(defaultIncomeCategory);
            }

            return new List<HandlerServiceResult>
            {
                new HandlerServiceResult
                {
                    Message = HelpText,
                    StatusCode = StatusCodeEnum.Ok
                }
            };
        }
    }
}