using System;

namespace FinanceManager.Bot.DataAccessLayer.Models
{
    public class Operation : BaseModel
    {
        public long CreditAmountInCents { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }
    }
}