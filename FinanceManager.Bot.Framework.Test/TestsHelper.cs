using System;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.Helpers.Models;

namespace FinanceManager.Bot.Framework.Test
{
    public static class TestsHelper
    {
        public static User GetUser(long chatId = long.MinValue, string firstName = "alex", string lastName = "kozlov", string id = "id")
        {
            return new User
            {
                ChatId = chatId,
                LastName = lastName,
                FirstName = firstName,
                Id = id,
                Context = new Context()
            };
        }

        public static Message GetMessage(long chatId = long.MinValue, string text = "text")
        {
            return new Message
            {
                ChatInfo = new ChatInfo
                {
                    Id = chatId
                },
                UserInfo = new UserInfo
                {
                    ChatId  = chatId
                },
                Text = text
            };
        }
    }
}