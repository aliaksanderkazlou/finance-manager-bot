using System;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.DataAccessLayer.Services.UnhandledMessages;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FinanceManager.Bot.Framework.Services
{
    public sealed class UnhandledMessageService
    {
        private const string ErrorText = "Sorry, I cannot handle this command.";

        private readonly IUnhandledMessageDocumentService _unhandledMessageDocumentService;

        public UnhandledMessageService(IUnhandledMessageDocumentService unhandledMessageDocumentService)
        {
            _unhandledMessageDocumentService = unhandledMessageDocumentService;
        }

        public async Task<HandlerServiceResult> Handle(Message message)
        {
            await _unhandledMessageDocumentService.InsertAsync(new UnhandledMessage
            {
                ChatId = message.Chat.Id,
                Created = DateTime.Now,
                Text = message.Text
            });

            return new HandlerServiceResult()
            {
                Message = ErrorText,
                StatusCode = StatusCodeEnum.Bad
            };
        }
    }
}