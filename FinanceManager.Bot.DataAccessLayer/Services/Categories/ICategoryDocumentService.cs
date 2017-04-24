using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;

namespace FinanceManager.Bot.DataAccessLayer.Services.Categories
{
    public interface ICategoryDocumentService : IDocumentService<Category>
    {
        Task<List<Category>> GetByUserId(string id);

        Task<List<Category>> GetByName(string name);
    }
}