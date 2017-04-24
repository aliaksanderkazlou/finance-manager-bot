using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.DataAccessLayer.Services.Categories;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;
using FinanceManager.Bot.Helpers.Enums;
using User = FinanceManager.Bot.DataAccessLayer.Models.User;
using Message = FinanceManager.Bot.Helpers.Models.Message;

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
                {QuestionsEnum.CategoryOperation, ConfigureCategoryOperation },
                {QuestionsEnum.CategorySupposedToSpentThisMonth, ConfigureCategorySupposedToSpentThisMonth },
                {QuestionsEnum.CategoryType, ConfigureCategoryType },
                {QuestionsEnum.AddNewCategoryOrNot, ConfigureCategoryCreating },
                {QuestionsEnum.CategoryName, ConfigureCategoryName},
                {QuestionsEnum.DeleteCategory, ConfigureCategoryDelete },
                {QuestionsEnum.EditCategory, ConfigureCategoryEdit }
            };
        }

        private async Task<HandlerServiceResult> ConfigureCategoryEdit(string answer, User user)
        {
            answer = answer.Trim();

            var categories = await _categoryDocumentService.GetByUserId(user.Id);

            var categoryToEdit = categories.FirstOrDefault(c => c.Name.Equals(answer));

            if (categoryToEdit != null)
            {
                user.Context.LastQuestion = QuestionsEnum.CategoryName;

                user.Context.CategoryId = categoryToEdit.Id;

                await _userDocumentService.UpdateAsync(user);

                return new HandlerServiceResult
                {
                    Message = "Great! Please, type name of your category.",
                    StatusCode = StatusCodeEnum.Ok
                };
            }

            user.Context.LastQuestion = QuestionsEnum.None;

            user.Context.CategoryId = null;

            await _userDocumentService.UpdateAsync(user);

            return new HandlerServiceResult
            {
                Message = "Sorry, you don't have any category with this name.",
                StatusCode = StatusCodeEnum.Ok
            };
        }

        private async Task<HandlerServiceResult> ConfigureCategoryDelete(string answer, User user)
        {
            answer = answer.Trim();

            var categories = await _categoryDocumentService.GetByUserId(user.Id);

            var categoryToDelete = categories.FirstOrDefault(c => c.Name.Equals(answer));

            if (categoryToDelete != null)
            {
                user.Context.LastQuestion = QuestionsEnum.None;

                user.Context.CategoryId = null;

                await _userDocumentService.UpdateAsync(user);

                await _categoryDocumentService.DeleteAsync(categoryToDelete.Id);

                return new HandlerServiceResult
                {
                    Message = string.Format("Category {0} was deleted successfully.", categoryToDelete.Name),
                    StatusCode = StatusCodeEnum.Ok
                };
            }

            user.Context.LastQuestion = QuestionsEnum.None;

            user.Context.CategoryId = null;

            await _userDocumentService.UpdateAsync(user);

            return new HandlerServiceResult
            {
                Message = "Sorry, you don't have any category with this name.",
                StatusCode = StatusCodeEnum.Ok
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

            var categoriesWithTheSameName = await _categoryDocumentService.GetByName(answer);

            if (categoriesWithTheSameName.Count > 0)
            {
                return new HandlerServiceResult
                {
                    Message = "Sorry, name should be unique.",
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
                    Message = "Great! Please, type name of your category.",
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

        private async Task<HandlerServiceResult> ConfigureCategoryOperation(string answer, User user)
        {
            if (string.IsNullOrEmpty(answer) || !answer.Contains("Add new category") && !answer.Contains("Edit category") && !answer.Contains("Delete category"))
            {
                return new HandlerServiceResult
                {
                    Helper = new List<string>
                    {
                        "Add new category",
                        "Edit category",
                        "Delete category"
                    },
                    Message = "Sorry, you should chose one of three options, or you can /cancel command.",
                    StatusCode = StatusCodeEnum.NeedKeyboard
                };
            }

            answer = answer.Trim();

            if (answer.Equals("Add new category"))
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

            if (answer.Equals("Edit category"))
            {
                user.Context.LastQuestion = QuestionsEnum.EditCategory;

                await _userDocumentService.UpdateAsync(user);

                var categories = await _categoryDocumentService.GetByUserId(user.Id);

                return new HandlerServiceResult
                {
                    Message = "Please, choose category to edit.",
                    StatusCode = StatusCodeEnum.NeedKeyboard,
                    Helper = categories.Select(c => c.Name).ToList()
                };
            }
            else
            {
                user.Context.LastQuestion = QuestionsEnum.DeleteCategory;

                await _userDocumentService.UpdateAsync(user);

                var categories = await _categoryDocumentService.GetByUserId(user.Id);

                return new HandlerServiceResult
                {
                    Message = "Please, choose category to delete.",
                    StatusCode = StatusCodeEnum.NeedKeyboard,
                    Helper = categories.Select(c => c.Name).ToList()
                };
            }
        }

        private async Task<HandlerServiceResult> ConfigureCategorySupposedToSpentThisMonth(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer) || !long.TryParse(answer, out long number) || number <= 0)
            {
                return new HandlerServiceResult
                {
                    Message = "Sorry, you can type only number greater than 0, or you can /cancel command.",
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
                Message = "Your category configured. Now you can create operations in this category by using /category command.",
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
            var userSearchResult = await _userDocumentService.GetByChatId(message.UserInfo.ChatId);

            var user = userSearchResult.FirstOrDefault();

            if (user == null)
            {
                user = new User
                {
                    Id = _userDocumentService.GenerateNewId(),
                    ChatId = message.UserInfo.ChatId,
                    FirstName = message.UserInfo.FirstName,
                    LastName = message.UserInfo.LastName
                };

                await _userDocumentService.InsertAsync(user);
            }

            var categories = await _categoryDocumentService.GetByUserId(user.Id);

            if (categories != null && categories.Count > 0)
            {
                user.Context = new Context
                {
                    LastQuestion = QuestionsEnum.CategoryOperation
                };

                await _userDocumentService.UpdateAsync(user);

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
    }
}