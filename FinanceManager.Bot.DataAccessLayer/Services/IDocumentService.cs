using System.Threading.Tasks;

namespace FinanceManager.Bot.DataAccessLayer.Services
{
    public interface IDocumentService<T>
    {
        Task CreateAsync(T item);

        void Update(T item);

        void Delete(int id);

        T GetById(int id);
    }
}