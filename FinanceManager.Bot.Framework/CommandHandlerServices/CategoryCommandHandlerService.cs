using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.DataAccessLayer.Services.Categories;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;
using FinanceManager.Bot.Helpers.Enums;
using Telegram.Bot.Types;
using User = FinanceManager.Bot.DataAccessLayer.Models.User;

namespace FinanceManager.Bot.Framework.CommandHandlerServices
{
    public class CategoryCommandHandlerService : ICommandHandlerService
    {
        private readonly IUserDocumentService _userDocumentService;
        private readonly ICategoryDocumentService _categoryDocumentService;
        private delegate Task<HandlerServiceResult> QuestionsHandlerDelegate(string answer, User user);
        private Dictionary<QuestionsEnum, QuestionsHandlerDelegate> _questionsHandlerDictionary;

        public CategoryCommandHandlerService(
            IUserDocumentService userDocumentService,
            ICategoryDocumentService categoryDocumentService)
        {
            _categoryDocumentService = categoryDocumentService;
            _userDocumentService = userDocumentService;
            InitializeQuestionsHandlerDictionary();
        }

        private void InitializeQuestionsHandlerDictionary()
        {
            _questionsHandlerDictionary = new Dictionary<QuestionsEnum, QuestionsHandlerDelegate>
            {
                {QuestionsEnum.CategoryCurrency, ConfigureCategoryCurrency },
                {QuestionsEnum.CategoryOperation, ConfigureCategoryOperation },
                {QuestionsEnum.CategorySupposedToSpentThisMonth, ConfigureCategorySupposedToSpentThisMonth },
                {QuestionsEnum.CategoryType, ConfigureCategoryType },
                {QuestionsEnum.AddNewCategoryOrNot, ConfigureCategoryCreating },
                {QuestionsEnum.CategoryName, ConfigureCategoryName}
            };
        }

        private async Task<HandlerServiceResult> ConfigureCategoryName(string answer, User user)
        {
            if (string.IsNullOrEmpty(answer))
            {
                return new HandlerServiceResult
                {
                    Message = "Sorry, you should type something",
                    StatusCode = StatusCodeEnum.Bad
                };
            }

            var category = await _categoryDocumentService.GetByIdAsync(user.Context.CategoryId);

            category.Name = answer;

            await _categoryDocumentService.UpdateAsync(category);

            user.Context.LastQuestion = QuestionsEnum.CategoryType;

            await _userDocumentService.UpdateAsync(user);

            return new HandlerServiceResult
            {
                StatusCode = StatusCodeEnum.NeedKeyboard,
                Helper = new List<string>
                {
                    "Income",
                    "Expense"
                },
                Message = "Now select the type of your category."
            };
        }

        private async Task<HandlerServiceResult> ConfigureCategoryCreating(string answer, User user)
        {
            if (string.IsNullOrEmpty(answer) || !answer.Contains("Yes") && !answer.Contains("No"))
                return new HandlerServiceResult
                {
                    Message = "Sorry, you can type only Yes or No, or you can /cancel command.",
                    Helper = new List<string>
                    {
                        "Yes",
                        "No"
                    },
                    StatusCode = StatusCodeEnum.NeedKeyboard
                };

            answer = answer.Trim();

            if (answer.Equals("Yes"))
            {
                var category = new Category
                {
                    Id = _categoryDocumentService.GenerateNewId(),
                    UserId = user.Id,
                    Operations = new List<Operation>()
                };

                await _categoryDocumentService.InsertAsync(category);

                user.Context.LastQuestion = QuestionsEnum.CategoryName;
                user.Context.CategoryId = category.Id;

                await _userDocumentService.UpdateAsync(user);

                return new HandlerServiceResult
                {
                    Message = "Great! Please, type name of your new category.",
                    StatusCode = StatusCodeEnum.Ok
                };
            }

            return new HandlerServiceResult
            {
                Message = "You always can create, edit or delete categories by /category command",
                StatusCode = StatusCodeEnum.Ok
            };
        }

        private async Task<HandlerServiceResult> ConfigureCategoryType(string answer, User user)
        {
            if (string.IsNullOrEmpty(answer) || !answer.Contains("Income") && !answer.Contains("Expense"))
                return new HandlerServiceResult
                {
                    Message = "Sorry, you can type only Income or Expense, or you can /cancel command",
                    Helper = new List<string>
                    {
                        "Income",
                        "Expense"
                    },
                    StatusCode = StatusCodeEnum.NeedKeyboard
                };

            answer = answer.Trim();

            var categoryType = answer.Equals("Income") ? CategoryTypeEnum.Income : CategoryTypeEnum.Expense;

            var category = await _categoryDocumentService.GetByIdAsync(user.Context.CategoryId);

            category.Type = categoryType;

            await _categoryDocumentService.UpdateAsync(category);

            user.Context.LastQuestion = QuestionsEnum.CategorySupposedToSpentThisMonth;

            await _userDocumentService.UpdateAsync(user);

            return new HandlerServiceResult
            {
                Message = string.Format("You choose {0}. Please, type the amount of money you would like to spend per month.", answer)
            };
        }

        private async Task<HandlerServiceResult> ConfigureCategoryCurrency(string answer, User user)
        {
            return new HandlerServiceResult();
        }

        private async Task<HandlerServiceResult> ConfigureCategoryOperation(string answer, User user)
        {
            return new HandlerServiceResult();
        }

        private async Task<HandlerServiceResult> ConfigureCategorySupposedToSpentThisMonth(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer) || !long.TryParse(answer, out long number) || number < 0)
            {
                return new HandlerServiceResult
                {
                    Message = "Sorry, you can type only number greater than 0, or you can /cancel command",
                    StatusCode = StatusCodeEnum.Bad
                };
            }

            var category = await _categoryDocumentService.GetByIdAsync(user.Context.CategoryId);

            category.SupposedToSpentThisMonthInCents = number;
            category.SpentThisMonthInCents = 0;
            category.SpentInCents = 0;

            await _categoryDocumentService.UpdateAsync(category);

            user.Context.LastQuestion = QuestionsEnum.None;
            user.Context.CategoryId = null;

            await _userDocumentService.UpdateAsync(user);

            return new HandlerServiceResult
            {
                Message = "You created category! Now you can create operations in this category.",
                StatusCode = StatusCodeEnum.Ok
            };
        }

        public async Task<HandlerServiceResult> HandleCategoryQuestion(
            string answer,
            User user)
        {
            HandlerServiceResult result;

            try
            {
                result = await _questionsHandlerDictionary[user.Context.LastQuestion].Invoke(answer, user);
            }
            catch (KeyNotFoundException)
            {
                result = new HandlerServiceResult
                {
                    StatusCode = StatusCodeEnum.Bad,
                    Message = "Sorry, something went wrong. Please, try /cancel the command and start again."
                };
            }

            return result;
        }

        public async Task<HandlerServiceResult> Handle(Message message)
        {
            var userSearchResult = await _userDocumentService.GetByChatId(message.Chat.Id);

            var user = userSearchResult.FirstOrDefault();

            if (user == null)
            {
                user = new User
                {
                    Id = _userDocumentService.GenerateNewId(),
                    ChatId = message.Chat.Id
                };

                await _userDocumentService.InsertAsync(user);
            }

            var categories = await _categoryDocumentService.GetByUserId(user.Id);

            if (categories != null && categories.Count > 0)
            {
                var categoriesString = string.Join("\n", categories.Select(c => c.Name));

                return new HandlerServiceResult
                {
                    Helper = new List<string>
                    {
                        "Add new category",
                        "Edit category",
                        "Delete category"
                    },
                    Message = string.Format("You have {0} categories. {1} You can add new, edit or delete category.",
                        categories.Count, categoriesString),
                    StatusCode = StatusCodeEnum.NeedKeyboard
                };
            }

            user.Context = new Context
            {
                LastQuestion = QuestionsEnum.AddNewCategoryOrNot
            };

            await _userDocumentService.UpdateAsync(user);

            return new HandlerServiceResult
            {
                StatusCode = StatusCodeEnum.NeedKeyboard,
                Helper = new List<string>
                {
                    "Yes",
                    "No"
                },
                Message = "You don't have any categories yet. Do you want to add categories?"
            };

        }

        //private HandlerServiceResult 
    }
}