using System.Collections.Generic;
using System.Linq;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;
using FinanceManager.Bot.Helpers.Enums;

namespace FinanceManager.Bot.Framework.Helpers
{
    public class QuestionHelper
    {
        private delegate HandlerServiceResult QuestionsHandlerDelegate();
        private Dictionary<QuestionsEnum, QuestionsHandlerDelegate> _questionsBuilderDictionary;
        private readonly ResultHelper _resultHelper;

        public QuestionHelper(
            ResultHelper resultHelper)
        {
            _resultHelper = resultHelper;
            InitializeQuestionBuilderDictionary();
        }

        public HandlerServiceResult BuildQuestion(QuestionsEnum question)
        {
            try
            {
                return _questionsBuilderDictionary[question].Invoke();
            }
            catch (KeyNotFoundException)
            {
                return _resultHelper.BuildErrorResult();
            }
        }

        private void InitializeQuestionBuilderDictionary()
        {
            _questionsBuilderDictionary = new Dictionary<QuestionsEnum, QuestionsHandlerDelegate>
            {
                {QuestionsEnum.OperationAction, BuildOperationActionQuestion},
                {QuestionsEnum.OperationCategory, BuildOperationCategoryQuestion},
                {QuestionsEnum.OperationDate, BuildOperationDateQuestion},
                {QuestionsEnum.OperationSum, BuildOperationSumQuestion},
                {QuestionsEnum.OperationType, BuildOperationTypeQuestion}
            };
        }

        private HandlerServiceResult BuildOperationTypeQuestion()
        {
            return new HandlerServiceResult();
        }

        private HandlerServiceResult BuildOperationSumQuestion()
        {
            return new HandlerServiceResult();
        }

        private HandlerServiceResult BuildOperationDateQuestion()
        {
            return new HandlerServiceResult();
        }

        private HandlerServiceResult BuildOperationCategoryQuestion()
        {
            return new HandlerServiceResult();
        }

        private HandlerServiceResult BuildOperationActionQuestion()
        {
            return new HandlerServiceResult
            {
                Message = "What would you like to do?",
                StatusCode = StatusCodeEnum.NeedKeyboard,
                Helper = new List<string>
                {
                    "Add operation",
                    "Delete operation"
                }
            };
        }

        public QuestionsEnum GetNextOperationQuestion(List<QuestionsEnum> questions)
        {
            return questions.Count > 0 ? questions.First() : QuestionsEnum.None;
        }

        public void RemoveOperationQuestionFromList(List<QuestionsEnum> questions, QuestionsEnum question)
        {
            questions.Remove(question);
        }

        public List<QuestionsEnum> GetOperationQuestions()
        {
            return new List<QuestionsEnum>
            {
                QuestionsEnum.OperationAction,
                QuestionsEnum.OperationType,
                QuestionsEnum.OperationCategory,
                QuestionsEnum.OperationSum,
                QuestionsEnum.OperationDate
            };
        }
    }
}