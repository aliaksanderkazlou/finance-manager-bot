using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.DataAccessLayer.Services.Categories;
using FinanceManager.Bot.DataAccessLayer.Services.Operations;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.Framework.Helpers;
using FinanceManager.Bot.Framework.Results;
using FinanceManager.Bot.Framework.Services;
using FinanceManager.Bot.Helpers.Enums;
using FinanceManager.Bot.Helpers.Models;
using FinanceManager.Bot.Helpers.Structures;

namespace FinanceManager.Bot.Framework.CommandHandlerServices
{
    public class OperationCommandHandlerService : ICommandHandlerService
    {
        private readonly IUserDocumentService _userDocumentService;
        private readonly ICategoryDocumentService _categoryDocumentService;
        private readonly IOperationDocumentService _operationDocumentService;
        private readonly ResultService _resultService;
        private readonly QuestionService _questionService;
        private readonly DocumentServiceHelper _documentServiceHelper;
        private delegate Task<List<HandlerServiceResult>> QuestionsHandlerDelegate(string answer, User user);
        private Dictionary<QuestionsEnum, QuestionsHandlerDelegate> _questionsHandlerDictionary;
        private QuestionTree _tree;

        public OperationCommandHandlerService(
            IUserDocumentService userDocumentService,
            ICategoryDocumentService categoryDocumentService,
            IOperationDocumentService operationDocumentService,
            ResultService resultService,
            QuestionService questionService,
            DocumentServiceHelper documentServiceHelper)
        {
            _userDocumentService = userDocumentService;
            _categoryDocumentService = categoryDocumentService;
            _operationDocumentService = operationDocumentService;
            _resultService = resultService;
            _questionService = questionService;
            _documentServiceHelper = documentServiceHelper;
            InitializeQuestionsHandlerDictionary();
            InitializeQuestionTree();
        }

        private void InitializeQuestionsHandlerDictionary()
        {
            _questionsHandlerDictionary = new Dictionary<QuestionsEnum, QuestionsHandlerDelegate>
            {
                {QuestionsEnum.OperationDate, ConfigureOperationDate },
                {QuestionsEnum.OperationSum, ConfigureOperationSum },
                {QuestionsEnum.OperationCategory, ConfigureOperationCategory}
            };
        }

        private void InitializeQuestionTree()
        {
            _tree = new QuestionTree
            {
                Question = QuestionsEnum.OperationCategory,
                Children = new List<QuestionTree>
                {
                    new QuestionTree
                    {
                        Question = QuestionsEnum.OperationSum,
                        Children = new List<QuestionTree>
                        {
                            new QuestionTree
                            {
                                Question = QuestionsEnum.OperationDate,
                                Children = new List<QuestionTree>()
                            }
                        }
                    }
                }
            };
        }

        private async Task<Operation> GetOrCreateOperationAsync(User user)
        {
            Operation operation;

            if (user.Context.OperationId == null)
            {
                operation = new Operation
                {
                    Id = _operationDocumentService.GenerateNewId(),
                    CategoryId = user.Context.CategoryId
                };

                await _documentServiceHelper.InsertOperationAsync(operation);
            }
            else
            {
                operation = await _documentServiceHelper.GetOperationByIdAsync(user.Context.OperationId);
            }

            return operation;
        }

        private async Task<List<HandlerServiceResult>> ConfigureOperationDate(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer) || !answer.Equals("now"))
            {
                return new List<HandlerServiceResult> { _resultService.BuildOperationInvalidDateErrorResult() };
            }

            DateTime date;

            if (answer.Equals("now"))
            {
                date = DateTime.Now;
            }
            else
            {
                var dayMonthYear = answer.Split('.');

                try
                {
                    date = new DateTime(int.Parse(dayMonthYear[2]), int.Parse(dayMonthYear[1]),
                        int.Parse(dayMonthYear[0]));
                }
                catch (Exception)
                {
                    return new List<HandlerServiceResult> {_resultService.BuildOperationInvalidDateErrorResult()};
                }
            }

            var operation = await GetOrCreateOperationAsync(user);

            operation.Date = date;
            operation.Configured = true;

            await _operationDocumentService.UpdateAsync(operation);

            user.Context = null;

            await _userDocumentService.UpdateAsync(user);
            
            return new List<HandlerServiceResult> {_resultService.BuildFinishedConfiguringOperationResult()};
        }

        private async Task<List<HandlerServiceResult>> ConfigureOperationSum(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer) || !decimal.TryParse(answer, out decimal sum))
            {
                return new List<HandlerServiceResult> { _resultService.BuildOperationInvalidSumErrorResult() };
            }

            var result = new List<HandlerServiceResult>();

            var operation = await GetOrCreateOperationAsync(user);

            var category = await _categoryDocumentService.GetByIdAsync(operation.CategoryId);

            var sumInCents = (long)sum * 100;

            if (category.Type == CategoryTypeEnum.Expense)
            {
                category.SpentInCents += sumInCents;
                category.SpentThisMonthInCents += sumInCents;

                if (category.SpentThisMonthInCents > category.SupposedToSpentThisMonthInCents)
                {
                    result.Add(_resultService.BuildOperationExceededAmountForThisMonth(
                        (float) (category.SpentThisMonthInCents - category.SupposedToSpentThisMonthInCents) / 100));
                }
            }

            operation.CreditAmountInCents = sumInCents;

            await _operationDocumentService.UpdateAsync(operation);

            await _categoryDocumentService.UpdateAsync(category);

            user.Context.CurrentNode = user.Context.CurrentNode.Children.FirstOrDefault();

            var nextQuestion = await _questionService.BuildQuestion(user);

            await _userDocumentService.UpdateAsync(user);

            result.Add(nextQuestion);

            return result;
        }

        private async Task<List<HandlerServiceResult>> ConfigureOperationType(CategoryTypeEnum type, User user)
        {
            HandlerServiceResult nextQuestion;

            var categories = await _categoryDocumentService.GetByUserIdAsync(user.Id);

            categories = categories.Where(c => c.Configured && c.Type == type).ToList();

            if (categories.Count > 0)
            {
                var operation = new Operation
                {
                    Configured = false,
                    Id = _operationDocumentService.GenerateNewId()
                };

                await _operationDocumentService.InsertAsync(operation);

                user.Context = new Context
                {
                    OperationId = operation.Id,
                    CategoryType = type,
                    CurrentNode = _tree
                };

                nextQuestion = await _questionService.BuildQuestion(user);
            }
            else
            {
                nextQuestion = _resultService.BuildOperationTypeCleanCategoryList();
            }

            await _userDocumentService.UpdateAsync(user);

            return new List<HandlerServiceResult> { nextQuestion };
        }

        private async Task<List<HandlerServiceResult>> ConfigureOperationCategory(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer))
            {
                return new List<HandlerServiceResult> {_resultService.BuildEmptyAnswerErrorResult()};
            }

            var userCategories = await _documentServiceHelper.GetUserCategoriesByTypeAsync(user);

            var category = userCategories.FirstOrDefault(c => c.Name.Equals(answer));

            if (category == null)
            {
                return new List<HandlerServiceResult>{_resultService.BuildOperationCategoryNotFoundErrorResult()};
            }

            user.Context.CategoryId = category.Id;

            var operation = await GetOrCreateOperationAsync(user);
            operation.CategoryId = category.Id;

            await _documentServiceHelper.UpdateOperationAsync(operation);

            user.Context.CurrentNode = user.Context.CurrentNode.Children.FirstOrDefault();

            var nextQuestion = await _questionService.BuildQuestion(user);

            await _userDocumentService.UpdateAsync(user);

            return new List<HandlerServiceResult> { nextQuestion };
        }

        public async Task<List<HandlerServiceResult>> HandleOperationQuestion(string answer, User user)
        {
            List<HandlerServiceResult> result;

            try
            {
                result = await _questionsHandlerDictionary[user.Context.CurrentNode.Question].Invoke(answer, user);
            }
            catch (KeyNotFoundException)
            {
                result = new List<HandlerServiceResult>{_resultService.BuildErrorResult()};
            }

            return result;
        }

        public async Task<List<HandlerServiceResult>> Handle(Message message)
        {
            var user = await _userDocumentService.GetByChatId(message.UserInfo.ChatId);

            var categoryType = message.Text.Equals("/income") ? CategoryTypeEnum.Income : CategoryTypeEnum.Expense;

            user.Context = new Context
            {
                CurrentNode = _tree
            };

            var result = await ConfigureOperationType(categoryType, user);

            await _userDocumentService.UpdateAsync(user);

            return result;
        }
    }
}