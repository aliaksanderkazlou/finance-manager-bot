using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FinanceManager.Bot.Framework.CommandHandlerServices
{
    public class HelpCommandHandlerService : ICommandHandlerService
    {
        private static string HelpText => "Here is a list of commands I can execute\n" +
                                          "/inline - inline\n" +
                                          "/help - Find out what I can do";

        private readonly ITelegramBotClient _botClient;

        public HelpCommandHandlerService(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task Handle(Message message)
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id, HelpText);
        }
    }
}