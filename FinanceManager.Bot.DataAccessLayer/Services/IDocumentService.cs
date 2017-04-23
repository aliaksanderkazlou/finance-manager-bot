using System.Threading.Tasks;

namespace FinanceManager.Bot.DataAccessLayer.Services
{
    public interface IDocumentService<T>
    {
        Task InsertAsync(T item);

        Task UpdateAsync(T item);

        Task DeleteAsync(string id);

        Task<T> GetByIdAsync(string id);

        string GenerateNewId();
    }
}