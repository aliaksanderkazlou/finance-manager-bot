using System;

namespace FinanceManager.Bot.DataAccessLayer.Models
{
    public class UserStatus : BaseModel
    {
        public string UserId { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActiveLastThirtyDays { get; set; }
    }
}