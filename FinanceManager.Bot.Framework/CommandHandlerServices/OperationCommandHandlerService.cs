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
    public class OperationCommandHandlerService : ICommandHandlerService
    {
        private readonly IUserDocumentService _userDocumentService;
        private readonly ICategoryDocumentService _categoryDocumentService;
        private delegate Task<HandlerServiceResult> QuestionsHandlerDelegate(string answer, User user);
        private Dictionary<QuestionsEnum, QuestionsHandlerDelegate> _questionsHandlerDictionary;

        public OperationCommandHandlerService(
            IUserDocumentService userDocumentService,
            ICategoryDocumentService categoryDocumentService)
        {
            _userDocumentService = userDocumentService;
            _categoryDocumentService = categoryDocumentService;
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

        private async Task<HandlerServiceResult> ConfigureOperationDate(string answer, User user)
        {
            return new HandlerServiceResult();
        }

        private async Task<HandlerServiceResult> ConfigureOperationSum(string answer, User user)
        {
            return new HandlerServiceResult();
        }

        private async Task<HandlerServiceResult> ConfigureOperationType(string answer, User user)
        {
            return new HandlerServiceResult();
        }

        private async Task<HandlerServiceResult> ConfigureOperationAction(string answer, User user)
        {
            return new HandlerServiceResult();
        }

        private async Task<HandlerServiceResult> ConfigureOperationCategory(string answer, User user)
        {
            return new HandlerServiceResult();
        }

        public async Task<HandlerServiceResult> HandleOperationQuestion(string answer, User user)
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
            var user = await _userDocumentService.GetByChatId(message.UserInfo.ChatId);

            user.Context.LastQuestion = QuestionsEnum.OperationAction;

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
    }
}