using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.Helpers.Enums;
using MongoDB.Driver;

namespace FinanceManager.Bot.DataAccessLayer.Services.Categories
{
    public class CategoryDocumentService : BaseDocumentService<Category>, ICategoryDocumentService
    {
        public CategoryDocumentService(MongoService mongo) : base(mongo) {}

        protected override IMongoCollection<Category> Items => MongoService.Categories;

        public async Task<List<Category>> GetByUserIdAsync(string id)
        {
            var filter = Builders<Category>.Filter.Eq(f => f.UserId, id);

            return await SearchByFilter(filter);
        }
    }
}