namespace FinanceManager.Bot.DataAccessLayer.Services
{
    public abstract class BaseDocumentService<T> : BaseMongoService<T>
    {
        protected MongoService MongoService;

        protected BaseDocumentService(MongoService mongo)
        {
            MongoService = mongo;
        }
    }
}