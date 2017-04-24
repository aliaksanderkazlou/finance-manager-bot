using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;
using FinanceManager.Bot.Helpers.Enums;
using Message = FinanceManager.Bot.Helpers.Models.Message;

namespace FinanceManager.Bot.Framework.CommandHandlerServices
{
    public class CancelCommandHandlerService : ICommandHandlerService
    {
        private readonly IUserDocumentService _userDocumentService;

        public CancelCommandHandlerService(
            IUserDocumentService userDocumentService)
        {
            _userDocumentService = userDocumentService;
        }

        public async Task<HandlerServiceResult> Handle(Message message)
        {
            var userSearchResult = await _userDocumentService.GetByChatId(message.UserInfo.ChatId);

            var user = userSearchResult.FirstOrDefault();

            user.Context.LastQuestion = QuestionsEnum.None;
            user.Context.CategoryId = null;
            user.Context.OperationId = null;

            await _userDocumentService.UpdateAsync(user);

            return new HandlerServiceResult
            {
                Message = "Command cancelled",
                StatusCode = StatusCodeEnum.Ok
            };
        }
    }
}