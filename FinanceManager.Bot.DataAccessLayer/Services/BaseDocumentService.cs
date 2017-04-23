using FinanceManager.Bot.DataAccessLayer.Models;

namespace FinanceManager.Bot.DataAccessLayer.Services
{
    public abstract class BaseDocumentService<T> : BaseMongoService<T> where T : BaseModel
    {
        protected MongoService MongoService;

        protected BaseDocumentService(MongoService mongo)
        {
            MongoService = mongo;
        }
    }
}