using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Services.Categories;
using FinanceManager.Bot.DataAccessLayer.Services.Operations;
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
        private readonly IOperationDocumentService _operationDocumentService;

        public CancelCommandHandlerService(
            IUserDocumentService userDocumentService,
            ICategoryDocumentService categoryDocumentService,
            IOperationDocumentService operationDocumentService)
        {
            _userDocumentService = userDocumentService;
            _categoryDocumentService = categoryDocumentService;
            _operationDocumentService = operationDocumentService;
        }

        public async Task<List<HandlerServiceResult>> Handle(Message message)
        {
            var user = await _userDocumentService.GetByChatId(message.UserInfo.ChatId);

            if (user.Context.CategoryId != null)
            {
                await _categoryDocumentService.DeleteAsync(user.Context.CategoryId);
            }
            if (user.Context.OperationId != null)
            {
                await _operationDocumentService.DeleteAsync(user.Context.OperationId);
            }

            //user.Context.LastQuestion = QuestionsEnum.None;
            user.Context.CategoryId = null;
            user.Context.OperationId = null;

            await _userDocumentService.UpdateAsync(user);

            return new List<HandlerServiceResult>
            {
                new HandlerServiceResult
                {
                    Message = "Command cancelled",
                    StatusCode = StatusCodeEnum.Ok
                }
            };
        }
    }
}