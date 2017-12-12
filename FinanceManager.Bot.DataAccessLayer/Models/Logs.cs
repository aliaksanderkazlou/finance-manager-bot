using System.Collections.Generic;

namespace FinanceManager.Bot.DataAccessLayer.Models
{
    public class Logs : BaseModel
    {
        public LogRequest Request { get; set; }

        public List<LogResponse> Responses { get; set; }
    }
}