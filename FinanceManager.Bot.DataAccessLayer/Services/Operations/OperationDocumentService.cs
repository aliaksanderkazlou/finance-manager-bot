using FinanceManager.Bot.DataAccessLayer.Models;
using MongoDB.Driver;

namespace FinanceManager.Bot.DataAccessLayer.Services.Operations
{
    public class OperationDocumentService : BaseDocumentService<Operation>, IOperationDocumentService
    {
        public OperationDocumentService(MongoService mongo) : base(mongo) {}

        protected override IMongoCollection<Operation> Items => MongoService.Operations;
    }
}