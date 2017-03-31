namespace FinanceManager.Bot.DataAccessLayer.Services.Users
{
    public interface IUserDocumentService<T> where T: class
    {
        void Create(T item);

        void Update(T item);

        void Delete(int id);

        T GetById(int id);
    }
}