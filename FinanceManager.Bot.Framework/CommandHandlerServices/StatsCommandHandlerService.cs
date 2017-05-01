using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.DataAccessLayer.Services.Categories;
using FinanceManager.Bot.DataAccessLayer.Services.Operations;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;
using FinanceManager.Bot.Framework.Services;
using FinanceManager.Bot.Helpers.Enums;
using FinanceManager.Bot.Helpers.Models;
using FinanceManager.Bot.Helpers.Structures;

namespace FinanceManager.Bot.Framework.CommandHandlerServices
{
    public class StatsCommandHandlerService : ICommandHandlerService
    {
        private readonly StatsService _statsService;
        private readonly IUserDocumentService _userDocumentService;
        private readonly QuestionService _questionService;
        private readonly ResultService _resultService;
        private QuestionTree _tree;
        private delegate Task<List<HandlerServiceResult>> QuestionsHandlerDelegate(string answer, User user);
        private Dictionary<QuestionsEnum, QuestionsHandlerDelegate> _questionsHandlerDictionary;
        private readonly ICategoryDocumentService _categoryDocumentService;
        private readonly IOperationDocumentService _operationDocumentService;

        public StatsCommandHandlerService(
            StatsService statsService,
            IUserDocumentService userDocumentService,
            QuestionService questionService,
            ResultService resultService,
            ICategoryDocumentService categoryDocumentService,
            IOperationDocumentService operationDocumentService)
        {
            _statsService = statsService;
            _userDocumentService = userDocumentService;
            _questionService = questionService;
            _resultService = resultService;
            _categoryDocumentService = categoryDocumentService;
            _operationDocumentService = operationDocumentService;
            InitializeQuestionTree();
            InitializeQuestionsHandlerDictionary();
        }

        private void InitializeQuestionsHandlerDictionary()
        {
            _questionsHandlerDictionary = new Dictionary<QuestionsEnum, QuestionsHandlerDelegate>
            {
                {QuestionsEnum.StatsAction, ConfigureStatsAction },
                {QuestionsEnum.StatsCategory, ConfigureStatsCategory },
                {QuestionsEnum.StatsCategoryDateRange, ConfigureCategoryDateRange }
            };
        }

        private async Task<List<HandlerServiceResult>> ConfigureCategoryDateRange(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer))
            {
                return new List<HandlerServiceResult>
                {
                    _resultService.BuildEmptyAnswerErrorResult()
                };
            }

            var category = await _categoryDocumentService.GetByIdAsync(user.Context.CategoryId);

            if (answer.Equals("All time"))
            {
                user.Context = null;

                await _userDocumentService.UpdateAsync(user);

                return await _statsService.BuildStatistics(category);
            }

            try
            {
                var dateStrings = answer.Split('-');
                var from = dateStrings[0].Split('.');
                var to = dateStrings[1].Split('.');

                var fromDate = new DateTime(int.Parse(from[2]), int.Parse(from[1]), int.Parse(from[0]));
                var toDate = new DateTime(int.Parse(to[2]), int.Parse(to[1]), int.Parse(to[0]));

                var operations = await _operationDocumentService.GetByCategoryId(category.Id);
                operations = operations.Where(o => o.Date >= fromDate && o.Date <= toDate).ToList();

                if (operations.Count == 0)
                {
                    return new List<HandlerServiceResult>
                    {
                        _resultService.BuildStatsNoOperationsOnDateRangeErrorResult()   
                    };
                }

                user.Context = null;
                await _userDocumentService.UpdateAsync(user);

                return await _statsService.BuildStatistics(category, fromDate, toDate);
            }
            catch (Exception)
            {
                return new List<HandlerServiceResult>
                {
                    _resultService.BuildOperationInvalidDateErrorResult()
                };
            }

        }

        private async Task<List<HandlerServiceResult>> ConfigureStatsCategory(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer))
            {
                return new List<HandlerServiceResult>
                {
                    _resultService.BuildEmptyAnswerErrorResult()
                };
            }

            var categories = await _categoryDocumentService.GetByUserIdAsync(user.Id);

            var category = categories.FirstOrDefault(c => c.Name.Equals(answer));

            if (category == null)
            {
                return new List<HandlerServiceResult>
                {
                    _resultService.BuildCategoryNotFoundErrorResult()
                };
            }

            user.Context.CategoryId = category.Id;
            user.Context.CurrentNode = user.Context.CurrentNode.Children.FirstOrDefault();

            await _userDocumentService.UpdateAsync(user);

            return new List<HandlerServiceResult>
            {
                await _questionService.BuildQuestion(user)
            };
        }

        private async Task<List<HandlerServiceResult>> ConfigureStatsAction(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer) ||
                !answer.Equals("All categories") &&
                !answer.Equals("Income only") &&
                !answer.Equals("Expense only") &&
                !answer.Equals("Custom category"))
            {
                return new List<HandlerServiceResult>
                {
                    _resultService.BuildStatsWrongArgumentErrorResult()
                };
            }

            if (answer.Equals("All categories"))
            {
                user.Context = null;
                await _userDocumentService.UpdateAsync(user);

                return await _statsService.BuildStatistics(user);
            }

            if (answer.Equals("Income only"))
            {
                var categories = await _categoryDocumentService.GetByUserIdAsync(user.Id);
                categories = categories.Where(c => c.Configured && c.Type == CategoryTypeEnum.Income).ToList();

                if (categories.Count == 0)
                {
                    return new List<HandlerServiceResult>
                    {
                        _resultService.BuildOperationTypeCleanCategoryList()
                    };
                }

                user.Context = null;
                await _userDocumentService.UpdateAsync(user);

                return await _statsService.BuildStatistics(user, CategoryTypeEnum.Income);
            }

            if (answer.Equals("Expense only"))
            {
                var categories = await _categoryDocumentService.GetByUserIdAsync(user.Id);
                categories = categories.Where(c => c.Configured && c.Type == CategoryTypeEnum.Expense).ToList();

                if (categories.Count == 0)
                {
                    return new List<HandlerServiceResult>
                    {
                        _resultService.BuildOperationTypeCleanCategoryList()
                    };
                }

                user.Context = null;
                await _userDocumentService.UpdateAsync(user);

                return await _statsService.BuildStatistics(user, CategoryTypeEnum.Expense);
            }

            user.Context.CurrentNode = user.Context.CurrentNode.Children.FirstOrDefault();

            await _userDocumentService.UpdateAsync(user);

            return new List<HandlerServiceResult>{await _questionService.BuildQuestion(user)};
        }

        public async Task<List<HandlerServiceResult>> HandleStatsQuestion(
            string answer,
            User user)
        {
            List<HandlerServiceResult> result;

            try
            {
                result = await _questionsHandlerDictionary[user.Context.CurrentNode.Question].Invoke(answer, user);
            }
            catch (KeyNotFoundException)
            {
                result = new List<HandlerServiceResult>
                {
                    new HandlerServiceResult
                    {
                        StatusCode = StatusCodeEnum.Bad,
                        Message = "Sorry, something went wrong. Please, try /cancel the command and start again."
                    }
                };
            }

            return result;
        }

        private void InitializeQuestionTree()
        {
            _tree = new QuestionTree
            {
                Question = QuestionsEnum.StatsAction,
                Children = new List<QuestionTree>
                {
                    new QuestionTree
                    {
                        Question = QuestionsEnum.StatsCategory,
                        Children = new List<QuestionTree>
                        {
                            new QuestionTree
                            {
                                Question = QuestionsEnum.StatsCategoryDateRange,
                                Children = new List<QuestionTree>()
                            }
                        }
                    }
                }
            };
        }

        public async Task<List<HandlerServiceResult>> Handle(Message message)
        {
            var user = await _userDocumentService.GetByChatId(message.UserInfo.ChatId);

            var categories = await _categoryDocumentService.GetByUserIdAsync(user.Id);

            if (categories.Count == 0)
            {
                return new List<HandlerServiceResult>
                {
                    _resultService.BuildCleanCategoryList()
                };
            }

            user.Context = new Context
            {
                CurrentNode = _tree
            };

            await _userDocumentService.UpdateAsync(user);

            return new List<HandlerServiceResult>{await _questionService.BuildQuestion(user)};
        }
    }
}