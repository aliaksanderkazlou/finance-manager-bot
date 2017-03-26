using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FinanceManager.Bot.Framework.Services
{
    public sealed class UnhandledMessageService
    {
        private const string ErrorText = "Sorry, I cannot handle this command.";

        private readonly ITelegramBotClient _botClient;

        public UnhandledMessageService(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task Handle(Message message)
        {
            // TODO: add to db

            await _botClient.SendTextMessageAsync(message.Chat.Id, ErrorText);
        }
    }
}