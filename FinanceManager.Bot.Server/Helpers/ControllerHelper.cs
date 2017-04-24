using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FinanceManager.Bot.Server.Helpers
{
    public static class ControllerHelper
    {
        public static ReplyKeyboardMarkup BuildKeyBoardMarkup(List<string> options)
        {
            var list = new List<KeyboardButton[]>();
            for (var index = 0; index < options.Count; index += 2)
            {
                if (index + 1 <= options.Count - 1)
                {
                    list.Add(new[]
                    {
                        new KeyboardButton(options[index]),
                        new KeyboardButton(options[index + 1])
                    });
                }
                else
                {
                    list.Add(new [] {new KeyboardButton(options[index])});
                }
            }
            var keyboard = list.ToArray();

            var replyKeyBoard = new ReplyKeyboardMarkup(keyboard)
            {
                OneTimeKeyboard = true
            };

            return replyKeyBoard;
        }
    }
}