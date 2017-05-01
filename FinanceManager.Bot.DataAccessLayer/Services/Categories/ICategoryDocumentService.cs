using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.Helpers.Enums;

namespace FinanceManager.Bot.DataAccessLayer.Services.Categories
{
    public interface ICategoryDocumentService : IDocumentService<Category>
    {
        Task<List<Category>> GetByUserIdAsync(string id);
    }
}