using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using MongoDB.Driver;

namespace FinanceManager.Bot.DataAccessLayer.Services.Operations
{
    public class OperationDocumentService : BaseDocumentService<Operation>, IOperationDocumentService
    {
        public OperationDocumentService(MongoService mongo) : base(mongo) {}

        protected override IMongoCollection<Operation> Items => MongoService.Operations;

        public async Task<List<Operation>> GetByCategoryId(string categoryId)
        {
            var filter = Builders<Operation>.Filter.Eq(f => f.CategoryId, categoryId);

            return await SearchByFilter(filter);
        }
    }
}