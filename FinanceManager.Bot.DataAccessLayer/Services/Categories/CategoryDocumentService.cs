using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using MongoDB.Driver;

namespace FinanceManager.Bot.DataAccessLayer.Services.Categories
{
    public class CategoryDocumentService : BaseDocumentService<Category>, ICategoryDocumentService
    {
        public CategoryDocumentService(MongoService mongo) : base(mongo) {}

        protected override IMongoCollection<Category> Items => MongoService.Categories;

        public async Task<List<Category>> GetByUserId(string id)
        {
            var filter = Builders<Category>.Filter.Eq(f => f.UserId, id);

            return await SearchByFilter(filter);
        }

        public async Task<List<Category>> GetByName(string name)
        {
            var filter = Builders<Category>.Filter.Eq(f => f.Name, name);

            return await SearchByFilter(filter);
        }
    }
}