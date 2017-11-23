//using System.Collections.Generic;
//using System.Linq;
//using Telegram.Bot.Types;
//using Telegram.Bot.Types.InlineKeyboardButtons;
//using Telegram.Bot.Types.ReplyMarkups;

//namespace FinanceManager.Bot.Server.Helpers
//{
//    public static class ControllerHelper
//    {
//        public static InlineKeyboardMarkup BuildKeyBoardMarkup(List<string> options)
//        {
//            var list = new List<InlineKeyboardButton[]>();
//            for (var index = 0; index < options.Count; index += 2)
//            {
//                if (index + 1 <= options.Count - 1)
//                {
//                    list.Add(new[]
//                    {
//                        new InlineKeyboardButton(options[index]),
//                        new InlineKeyboardButton(options[index + 1])
//                    });
//                }
//                else
//                {
//                    list.Add(new [] {new InlineKeyboardButton(options[index])});
//                }
//            }
//            var keyboard = list.ToArray();

//            var replyKeyBoard = new InlineKeyboardMarkup(keyboard);

//            return replyKeyBoard;
//        }
//    }
//}