using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FinanceManager.Bot.Server.Helpers
{
    public class ControllerHelper
    {
        public ReplyKeyboardMarkup BuildKeyBoardMarkup(List<string> options)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new [] // first row
                {
                    new KeyboardButton("1.1"),
                    new KeyboardButton("1.2")
                },
                new [] // last row
                {
                    new KeyboardButton("2.1"),
                    new KeyboardButton("2.2")
                }
            });

            return keyboard;
        }
    }
}