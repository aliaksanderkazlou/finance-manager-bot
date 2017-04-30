using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Helpers;
using FinanceManager.Bot.Framework.Results;
using FinanceManager.Bot.Helpers.Enums;

namespace FinanceManager.Bot.Framework.Services
{
    public class QuestionService
    {
        private delegate Task<HandlerServiceResult> QuestionsHandlerDelegate();
        private Dictionary<QuestionsEnum, QuestionsHandlerDelegate> _questionsBuilderDictionary;
        private readonly DocumentServiceHelper _documentServiceHelper;
        private readonly ResultService _resultService;
        private User _user;

        public QuestionService(
            ResultService resultService,
            DocumentServiceHelper documentServiceHelper)
        {
            _resultService = resultService;
            _documentServiceHelper = documentServiceHelper;
            InitializeQuestionBuilderDictionary();
        }

        public async Task<HandlerServiceResult> BuildQuestion(User user)
        {
            _user = user;

            try
            {
                return await _questionsBuilderDictionary[user.Context.CurrentNode.Question].Invoke();
            }
            catch (KeyNotFoundException)
            {
                return _resultService.BuildErrorResult();
            }
        }

        private void InitializeQuestionBuilderDictionary()
        {
            _questionsBuilderDictionary = new Dictionary<QuestionsEnum, QuestionsHandlerDelegate>
            {
                {QuestionsEnum.OperationCategory, BuildOperationCategoryQuestion},
                {QuestionsEnum.OperationDate, BuildOperationDateQuestion},
                {QuestionsEnum.OperationSum, BuildOperationSumQuestion},
                {QuestionsEnum.OperationType, BuildOperationTypeQuestion},
                {QuestionsEnum.CategoryAction, BuildCategoryActionQuestion },
                {QuestionsEnum.AddNewCategoryOrNot, BuildAddCategoryOrNotQuestion },
                {QuestionsEnum.CategoryName, BuildCategoryNameQuestion},
                {QuestionsEnum.ChooseCategoryToEdit, BuildCategoryToEditQuestion },
                {QuestionsEnum.ChooseCategoryToDelete, BuildCategoryToDeleteQuestion },
                {QuestionsEnum.CategoryType, BuildCategoryTypeQuestion },
                {QuestionsEnum.CategorySupposedToSpentThisMonth, CategorySupposedToSpentThisMonthQuestion }
            };
        }

        private Task<HandlerServiceResult> CategorySupposedToSpentThisMonthQuestion()
        {
            return Task.FromResult(new HandlerServiceResult
            {
                Message = "Please, type the amount of money you would like to spend per month.",
                StatusCode = StatusCodeEnum.Ok
            });
        }

        private Task<HandlerServiceResult> BuildCategoryTypeQuestion()
        {
            return Task.FromResult(new HandlerServiceResult
            {
                Message = "Please, chose a type of category.",
                StatusCode = StatusCodeEnum.NeedKeyboard,
                Helper = new List<string>
                {
                    "Income",
                    "Expense"
                }
            });
        }

        private async Task<HandlerServiceResult> BuildCategoryToDeleteQuestion()
        {
            var categories = await _documentServiceHelper.GetUserCategories(_user.Id);

            return new HandlerServiceResult
            {
                Message = "Please, chose category to delete.",
                StatusCode = StatusCodeEnum.NeedKeyboard,
                Helper = categories.Where(c => c.Configured).Select(c => c.Name).ToList()
            };
        }

        private async Task<HandlerServiceResult> BuildCategoryToEditQuestion()
        {
            var categories = await _documentServiceHelper.GetUserCategories(_user.Id);

            return new HandlerServiceResult
            {
                Message = "Please, chose category to edit.",
                StatusCode = StatusCodeEnum.NeedKeyboard,
                Helper = categories.Where(c => c.Configured).Select(c => c.Name).ToList()
            };
        }

        private Task<HandlerServiceResult> BuildCategoryNameQuestion()
        {
            return Task.FromResult(new HandlerServiceResult
            {
                Message = "Please, type category name.",
                StatusCode = StatusCodeEnum.Ok
            });
        }

        private Task<HandlerServiceResult> BuildAddCategoryOrNotQuestion()
        {
            return Task.FromResult(new HandlerServiceResult
            {
                StatusCode = StatusCodeEnum.NeedKeyboard,
                Helper = new List<string>
                {
                    "Yes",
                    "No"
                },
                Message = "You don't have any categories yet. Do you want to add categories?"
            });
        }

        private async Task<HandlerServiceResult> BuildCategoryActionQuestion()
        {
            var categories = await _documentServiceHelper.GetUserCategories(_user.Id);

            categories = categories.Where(c => c.Configured).ToList();

            var categoriesString = string.Join("\n", categories.Select(c => c.Name));

            return new HandlerServiceResult
            {
                Helper = new List<string>
                {
                    "Add new category",
                    "Edit category",
                    "Delete category"
                },
                Message = $"Here's your categories list:\n{categoriesString}\nYou can add new, edit or delete category.",
                StatusCode = StatusCodeEnum.NeedKeyboard
            };
        }

        private Task<HandlerServiceResult> BuildOperationTypeQuestion()
        {
            return Task.FromResult(new HandlerServiceResult
            {
                Message = "Please, choose operation type",
                StatusCode = StatusCodeEnum.NeedKeyboard,
                Helper = new List<string>
                {
                    "+",
                    "-"
                }
            });
        }

        private Task<HandlerServiceResult> BuildOperationSumQuestion()
        {
            return Task.FromResult(new HandlerServiceResult
            {
                Message = "Please, enter the sum of operation",
                StatusCode = StatusCodeEnum.Ok
            });
        }

        private Task<HandlerServiceResult> BuildOperationDateQuestion()
        {
            return Task.FromResult(new HandlerServiceResult
            {
                Message = "Please, enter the date.",
                StatusCode = StatusCodeEnum.Ok
            });
        }

        private async Task<HandlerServiceResult> BuildOperationCategoryQuestion()
        {
            var categories = await _documentServiceHelper.GetUserCategoriesByTypeAsync(_user);

            return new HandlerServiceResult
            {
                Message = "Please, choose the category.",
                StatusCode = StatusCodeEnum.NeedKeyboard,
                Helper = categories.Select(c => c.Name).ToList()
            };
        }

        public List<QuestionsEnum> GetOperationQuestions()
        {
            return new List<QuestionsEnum>
            {
                QuestionsEnum.OperationType,
                QuestionsEnum.OperationCategory,
                QuestionsEnum.OperationSum,
                QuestionsEnum.OperationDate
            };
        }
    }
}