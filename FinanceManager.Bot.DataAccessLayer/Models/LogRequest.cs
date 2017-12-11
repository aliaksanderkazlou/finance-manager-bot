namespace FinanceManager.Bot.DataAccessLayer.Models
{
    public class LogRequest
    {
        public string Message { get; set; }

        public string UserId { get; set; }

        public long ChatId { get; set; }

        public Context Context { get; set; }
    }
}