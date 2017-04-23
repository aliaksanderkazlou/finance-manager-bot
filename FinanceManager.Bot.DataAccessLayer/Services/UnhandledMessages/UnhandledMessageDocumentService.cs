using FinanceManager.Bot.DataAccessLayer.Models;
using MongoDB.Driver;

namespace FinanceManager.Bot.DataAccessLayer.Services.UnhandledMessages
{
    public class UnhandledMessageDocumentService : BaseDocumentService<UnhandledMessage>, IUnhandledMessageDocumentService
    {
        public UnhandledMessageDocumentService(MongoService mongo) : base(mongo) {}

        protected override IMongoCollection<UnhandledMessage> Items => MongoService.UnhandledMessages;
    }
}