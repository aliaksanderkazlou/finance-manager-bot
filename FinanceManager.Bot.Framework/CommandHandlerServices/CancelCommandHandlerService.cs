using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Services.Categories;
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
        private readonly ICategoryDocumentService _categoryDocumentService;

        public CancelCommandHandlerService(
            IUserDocumentService userDocumentService,
            ICategoryDocumentService categoryDocumentService)
        {
            _userDocumentService = userDocumentService;
            _categoryDocumentService = categoryDocumentService;
        }

        public async Task<HandlerServiceResult> Handle(Message message)
        {
            var user = await _userDocumentService.GetByChatId(message.UserInfo.ChatId);

            if (user.Context.CategoryId != null)
            {
                await _categoryDocumentService.DeleteAsync(user.Context.CategoryId);
            }

            //TODO: delete operation

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