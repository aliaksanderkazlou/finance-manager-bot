using System;

namespace FinanceManager.Bot.DataAccessLayer.Models
{
    public class UnhandledMessage
    {
        public string Text { get; set; }

        public long ChatId { get; set; }

        public DateTime Created { get; set; }
    }
}