using System.Collections.Generic;
using System.Linq;
using FinanceManager.Bot.Helpers.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Message = Telegram.Bot.Types.Message;

namespace FinanceManager.Bot.Server.Helpers
{
    public static class ControllerHelper
    {
        public static InlineKeyboardMarkup BuildKeyBoardMarkup(List<string> options)
        {
            var list = new List<InlineKeyboardButton[]>();
            for (var index = 0; index < options.Count; index += 2)
            {
                if (index + 1 <= options.Count - 1)
                {
                    list.Add(new[]
                    {
                        new InlineKeyboardButton(options[index]),
                        new InlineKeyboardButton(options[index + 1])
                    });
                }
                else
                {
                    list.Add(new[] { new InlineKeyboardButton(options[index]) });
                }
            }
            var keyboard = list.ToArray();

            var replyKeyBoard = new InlineKeyboardMarkup(keyboard);

            return replyKeyBoard;
        }

        public static Bot.Helpers.Models.Message GetMessageFromMessageStructure(Message message)
        {
            return new Bot.Helpers.Models.Message()
            {
                Text = message.Text,
                UserInfo = new UserInfo
                {
                    FirstName = message.From.FirstName,
                    LastName = message.From.LastName,
                    ChatId = message.Chat.Id
                },
                ChatInfo = new ChatInfo
                {
                    Type = message.Chat.Type.ToString(),
                    Id = message.Chat.Id,
                    UserName = message.Chat.Username
                }
            };
        }

        public static Bot.Helpers.Models.Message GetMessageFromUpdateStructure(Update update)
        {
            return new Bot.Helpers.Models.Message()
            {
                Text = update.CallbackQuery.Data,
                UserInfo = new UserInfo()
                {
                    FirstName = update.CallbackQuery.From.FirstName,
                    LastName = update.CallbackQuery.From.LastName,
                    ChatId = update.CallbackQuery.From.Id
                },
                ChatInfo = new ChatInfo
                {
                    UserName = update.CallbackQuery.Message.Chat.Username,
                    Id = update.CallbackQuery.Message.Chat.Id,
                    Type = update.CallbackQuery.Message.Chat.Type.ToString()
                }
            };
        }
    }
}