using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;

namespace FinanceManager.Bot.DataAccessLayer.Services.Operations
{
    public interface IOperationDocumentService : IDocumentService<Operation>
    {
        Task<List<Operation>> GetByCategoryId(string categoryId);
    }
}