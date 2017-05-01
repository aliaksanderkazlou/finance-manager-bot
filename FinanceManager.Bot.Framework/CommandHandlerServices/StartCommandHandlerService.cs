using System.Collections.Generic;
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
        private const string HelpText = "/category - Add, edit or delete categories\n" +
                                        "/income - Add an income operation\n" +
                                        "/expense - Add an expense operation\n" +
                                        "/stats - Get statistics\n" +
                                        "/help - Find out what I can do\n" +
                                        "/cancel - Cancel the current command";

        public StartCommandHandlerService(
            IUserDocumentService userDocumentService,
            ICategoryDocumentService categoryDocumentService)
        {
            _userDocumentService = userDocumentService;
            _categoryDocumentService = categoryDocumentService;
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