using MongoDB.Driver;

namespace FinanceManager.Bot.DataAccessLayer.Services.Logs
{
    public class LogDocumentService : BaseDocumentService<Models.Logs>, ILogDocumentService
    {
        public LogDocumentService(MongoService mongo) : base(mongo)
        {
        }

        protected override IMongoCollection<Models.Logs> Items => MongoService.Logs;
    }
}