using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.DataAccessLayer.Services.Categories;
using FinanceManager.Bot.DataAccessLayer.Services.Operations;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Helpers;
using FinanceManager.Bot.Framework.Results;
using FinanceManager.Bot.Helpers.Enums;
using FinanceManager.Bot.Helpers.Models;

namespace FinanceManager.Bot.Framework.CommandHandlerServices
{
    public class OperationCommandHandlerService : ICommandHandlerService
    {
        private readonly IUserDocumentService _userDocumentService;
        private readonly ICategoryDocumentService _categoryDocumentService;
        private readonly IOperationDocumentService _operationDocumentService;
        private readonly ResultHelper _resultHelper;
        private readonly QuestionHelper _questionHelper;
        private delegate Task<List<HandlerServiceResult>> QuestionsHandlerDelegate(string answer, User user);
        private Dictionary<QuestionsEnum, QuestionsHandlerDelegate> _questionsHandlerDictionary;

        public OperationCommandHandlerService(
            IUserDocumentService userDocumentService,
            ICategoryDocumentService categoryDocumentService,
            IOperationDocumentService operationDocumentService,
            ResultHelper resultHelper,
            QuestionHelper questionHelper)
        {
            _userDocumentService = userDocumentService;
            _categoryDocumentService = categoryDocumentService;
            _operationDocumentService = operationDocumentService;
            _resultHelper = resultHelper;
            _questionHelper = questionHelper;
            InitializeQuestionsHandlerDictionary();
        }

        private void InitializeQuestionsHandlerDictionary()
        {
            _questionsHandlerDictionary = new Dictionary<QuestionsEnum, QuestionsHandlerDelegate>
            {
                {QuestionsEnum.OperationDate, ConfigureOperationDate },
                {QuestionsEnum.OperationSum, ConfigureOperationSum },
                {QuestionsEnum.OperationType, ConfigureOperationType },
                {QuestionsEnum.OperationAction, ConfigureOperationAction },
                {QuestionsEnum.OperationCategory, ConfigureOperationCategory}
            };
        }

        private async Task<List<HandlerServiceResult>> ConfigureOperationDate(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer) || !DateTime.TryParse(answer, out DateTime date))
            {
                return new List<HandlerServiceResult>{_resultHelper.BuildOperationInvalidDateErrorResult()};
            }

            var operation = user.Context.OperationId != null
                ? await _operationDocumentService.GetByIdAsync(user.Context.OperationId)
                : new Operation
                {
                    Id = _operationDocumentService.GenerateNewId(),
                    CategoryId = user.Context.CategoryId
                };

            operation.Date = date;

            await _operationDocumentService.UpdateAsync(operation);

            _questionHelper.RemoveOperationQuestionFromList(user.Context.Questions, user.Context.LastQuestion);
            user.Context.LastQuestion = _questionHelper.GetNextOperationQuestion(user.Context.Questions);

            if (user.Context.LastQuestion == QuestionsEnum.None)
            {
                user.Context.CategoryId = null;
                user.Context.OperationId = null;
                user.Context.Questions = null;

                await _userDocumentService.UpdateAsync(user);

                return new List<HandlerServiceResult>{_resultHelper.BuildFinishedOperationConfiguringResult()};
            }

            await _userDocumentService.UpdateAsync(user);

            return new List<HandlerServiceResult>{_questionHelper.BuildQuestion(user.Context.LastQuestion)};
        }

        private async Task<List<HandlerServiceResult>> ConfigureOperationSum(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer) || !decimal.TryParse(answer, out decimal sum))
            {
                return new List<HandlerServiceResult> {_resultHelper.BuildOperationInvalidSumErrorResult()};
            }

            var result = new List<HandlerServiceResult>();

            var operation = await _operationDocumentService.GetByIdAsync(user.Context.OperationId);

            var category = await _categoryDocumentService.GetByIdAsync(operation.CategoryId);

            var sumInCents = (long) sum * 100;

            category.SpentInCents += sumInCents;
            category.SpentThisMonthInCents += sumInCents;

            if (category.SpentThisMonthInCents > category.SupposedToSpentThisMonthInCents)
            {
                result.Add(_resultHelper.BuildOperationExceededAmountForThisMonth((float) (category.SpentThisMonthInCents - category.SupposedToSpentThisMonthInCents) / 100));
            }

            operation.CreditAmountInCents = sumInCents;

            await _operationDocumentService.UpdateAsync(operation);

            category.SpentInCents += sumInCents;
            category.SpentThisMonthInCents += sumInCents;

            await _categoryDocumentService.UpdateAsync(category);

            _questionHelper.RemoveOperationQuestionFromList(user.Context.Questions, user.Context.LastQuestion);
            user.Context.LastQuestion = _questionHelper.GetNextOperationQuestion(user.Context.Questions);

            if (user.Context.LastQuestion == QuestionsEnum.None)
            {
                user.Context.CategoryId = null;
                user.Context.OperationId = null;
                user.Context.Questions = null;

                await _userDocumentService.UpdateAsync(user);

                result.Add(_resultHelper.BuildFinishedOperationConfiguringResult());

                return result;
            }

            await _userDocumentService.UpdateAsync(user);

            result.Add(_questionHelper.BuildQuestion(user.Context.LastQuestion));

            return result;
        }

        private async Task<List<HandlerServiceResult>> ConfigureOperationType(string answer, User user)
        {
            return new List<HandlerServiceResult>();
        }

        private async Task<List<HandlerServiceResult>> ConfigureOperationAction(string answer, User user)
        {
            return new List<HandlerServiceResult>();
        }

        private async Task<List<HandlerServiceResult>> ConfigureOperationCategory(string answer, User user)
        {
            return new List<HandlerServiceResult>();
        }

        public async Task<List<HandlerServiceResult>> HandleOperationQuestion(string answer, User user)
        {
            List<HandlerServiceResult> result;

            try
            {
                result = await _questionsHandlerDictionary[user.Context.LastQuestion].Invoke(answer, user);
            }
            catch (KeyNotFoundException)
            {
                result = new List<HandlerServiceResult>{_resultHelper.BuildErrorResult()};
            }

            return result;
        }

        public async Task<List<HandlerServiceResult>> Handle(Message message)
        {
            var user = await _userDocumentService.GetByChatId(message.UserInfo.ChatId);

            user.Context.Questions = _questionHelper.GetOperationQuestions();
            user.Context.LastQuestion = _questionHelper.GetNextOperationQuestion(user.Context.Questions);

            await _userDocumentService.UpdateAsync(user);

            return new List<HandlerServiceResult> {_questionHelper.BuildQuestion(user.Context.LastQuestion) };
        }
    }
}