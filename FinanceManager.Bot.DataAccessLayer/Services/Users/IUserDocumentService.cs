using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;

namespace FinanceManager.Bot.DataAccessLayer.Services.Users
{
    public interface IUserDocumentService: IDocumentService<User>
    {
        Task<User> GetByChatId(long chatId);
    }
}