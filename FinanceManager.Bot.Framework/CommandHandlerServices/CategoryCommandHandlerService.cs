using System;
using System.Collections.Generic;
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
            var words = message.Text.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 1)
            {
                await _userDocumentService.InsertAsync(new User
                {
                    Id = message.Chat.Id.ToString(),
                    Categories = new List<Category>()
                });
            }

            return new HandlerServiceResult
            {
                StatusCode = StatusCodeEnum.Ok,
                Message = "You don't have any categories yet. Do you want to add categories?"
            };
        }

        //private HandlerServiceResult 
    }
}