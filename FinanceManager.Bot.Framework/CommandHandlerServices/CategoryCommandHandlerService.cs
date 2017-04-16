using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;
using Telegram.Bot.Types;
using User = FinanceManager.Bot.DataAccessLayer.Models.User;

namespace FinanceManager.Bot.Framework.CommandHandlerServices
{
    public class CategoryCommandHandlerService : ICommandHandlerService
    {
        private readonly IUserDocumentService _userDocumentService;

        public CategoryCommandHandlerService(IUserDocumentService userDocumentService)
        {
            _userDocumentService = userDocumentService;
        }

        public async Task<HandlerServiceResult> Handle(Message message)
        {
            await _userDocumentService.CreateAsync(new User());

            return new HandlerServiceResult
            {
                StatusCode = StatusCodeEnum.Ok
            };
        }
    }
}