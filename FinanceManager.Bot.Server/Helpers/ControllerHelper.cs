using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FinanceManager.Bot.Server.Helpers
{
    public static class ControllerHelper
    {
        public static InlineKeyboardMarkup BuildKeyBoardMarkup(List<string> options)
        {
            var keyboard = options.Select(o => new[] {new InlineKeyboardButton(o)}).ToArray();

            var replyKeyBoard = new InlineKeyboardMarkup(keyboard);

            return replyKeyBoard;
        }
    }
}