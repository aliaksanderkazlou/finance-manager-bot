using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;

namespace FinanceManager.Bot.DataAccessLayer.Services.UserStatuses
{
    public interface IUserStatusDocumentService : IDocumentService<UserStatus>
    {
       Task<UserStatus> GetByUserId(string userId);
    }
}