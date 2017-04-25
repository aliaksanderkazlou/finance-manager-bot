using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        private QuestionsEnum GenerateNextQuestionAndRemoveFromList(User user)
        {
            var nextQuestion = GetNextOperationQuestion(user.Context.Questions);

            if (nextQuestion != QuestionsEnum.None)
            {
                RemoveOperationQuestionFromList(user.Context.Questions, nextQuestion);
            }

            return nextQuestion;
        }

        public async Task<HandlerServiceResult> BuildQuestion(User user)
        {
            _user = user;
            var question = GenerateNextQuestionAndRemoveFromList(user);

            user.Context.LastQuestion = question;

            if (question == QuestionsEnum.None)
            {
                await _documentServiceHelper.DeleteUserContextAsync(user);

                return _resultService.BuildFinishedConfiguringResult();
            }

            try
            {
                return await _questionsBuilderDictionary[question].Invoke();
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
                {QuestionsEnum.OperationType, BuildOperationTypeQuestion}
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

        private QuestionsEnum GetNextOperationQuestion(List<QuestionsEnum> questions)
        {
            return questions.Count > 0 ? questions.First() : QuestionsEnum.None;
        }

        private void RemoveOperationQuestionFromList(List<QuestionsEnum> questions, QuestionsEnum question)
        {
            questions.Remove(question);
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