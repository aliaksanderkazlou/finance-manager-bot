namespace FinanceManager.Bot.DataAccessLayer.Services
{
    public interface IDocumentService<T>
    {
        void Create(T item);

        void Update(T item);

        void Delete(int id);

        T GetById(int id);
    }
}