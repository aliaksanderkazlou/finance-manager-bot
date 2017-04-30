using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.DataAccessLayer.Services.Categories;
using FinanceManager.Bot.DataAccessLayer.Services.Operations;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Helpers;
using FinanceManager.Bot.Framework.Results;
using FinanceManager.Bot.Framework.Services;
using FinanceManager.Bot.Helpers.Enums;
using FinanceManager.Bot.Helpers.Models;

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
        }

        private void InitializeQuestionsHandlerDictionary()
        {
            _questionsHandlerDictionary = new Dictionary<QuestionsEnum, QuestionsHandlerDelegate>
            {
                {QuestionsEnum.OperationDate, ConfigureOperationDate },
                {QuestionsEnum.OperationSum, ConfigureOperationSum },
                {QuestionsEnum.OperationType, ConfigureOperationType },
                {QuestionsEnum.OperationCategory, ConfigureOperationCategory}
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

            if (string.IsNullOrEmpty(answer))
            {
                return new List<HandlerServiceResult> { _resultService.BuildOperationInvalidDateErrorResult() };
            }

            var dayMonthYear = answer.Split('.');

            DateTime date;

            try
            {
                date = new DateTime(int.Parse(dayMonthYear[2]), int.Parse(dayMonthYear[1]), int.Parse(dayMonthYear[0]));
            }
            catch (Exception)
            {
                return new List<HandlerServiceResult> {_resultService.BuildOperationInvalidDateErrorResult()};
            }

            var operation = await GetOrCreateOperationAsync(user);

            operation.Date = date;

            await _operationDocumentService.UpdateAsync(operation);

            var nextQuestion = await _questionService.BuildQuestion(user);

            if (nextQuestion.StatusCode == StatusCodeEnum.Bad)
            {
                await _documentServiceHelper.DeleteUserContextAsync(user);
            }

            await _userDocumentService.UpdateAsync(user);
            
            return new List<HandlerServiceResult> {nextQuestion};
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

            category.SpentInCents += sumInCents;
            category.SpentThisMonthInCents += sumInCents;

            if (category.SpentThisMonthInCents > category.SupposedToSpentThisMonthInCents)
            {
                result.Add(_resultService.BuildOperationExceededAmountForThisMonth((float)(category.SpentThisMonthInCents - category.SupposedToSpentThisMonthInCents) / 100));
            }

            operation.CreditAmountInCents = sumInCents;

            await _operationDocumentService.UpdateAsync(operation);

            category.SpentInCents += sumInCents;
            category.SpentThisMonthInCents += sumInCents;

            await _categoryDocumentService.UpdateAsync(category);

            var nextQuestion = await _questionService.BuildQuestion(user);

            if (nextQuestion.StatusCode == StatusCodeEnum.Bad)
            {
                await _documentServiceHelper.DeleteUserContextAsync(user);
            }

            await _userDocumentService.UpdateAsync(user);

            result.Add(nextQuestion);

            return result;
        }

        private async Task<List<HandlerServiceResult>> ConfigureOperationType(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer) || !answer.Contains("+") && !answer.Contains("-"))
            {
                return new List<HandlerServiceResult> { _resultService.BuildOperationInvalidTypeErrorResult() };
            }

            var operation = await GetOrCreateOperationAsync(user);

            user.Context.OperationId = operation.Id;

            user.Context.CategoryType = answer.Equals("+")
                ? CategoryTypeEnum.Income
                : CategoryTypeEnum.Expense;

            var nextQuestion = await _questionService.BuildQuestion(user);

            if (nextQuestion.StatusCode == StatusCodeEnum.Bad)
            {
                await _documentServiceHelper.DeleteUserContextAsync(user);
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

            var nextQuestion = await _questionService.BuildQuestion(user);

            if (nextQuestion.StatusCode == StatusCodeEnum.Bad)
            {
                await _documentServiceHelper.DeleteUserContextAsync(user);
            }

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

            //user.Context.Questions = _questionService.GetOperationQuestions();

            var result = new List<HandlerServiceResult>
            {
                await _questionService.BuildQuestion(user)
            };

            await _userDocumentService.UpdateAsync(user);

            return result;
        }
    }
}