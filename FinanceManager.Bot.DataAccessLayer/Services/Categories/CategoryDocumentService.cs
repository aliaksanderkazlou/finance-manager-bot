using FinanceManager.Bot.DataAccessLayer.Models;
using MongoDB.Driver;

namespace FinanceManager.Bot.DataAccessLayer.Services.Categories
{
    public class CategoryDocumentService : BaseDocumentService<Category>, ICategoryDocumentService
    {
        public CategoryDocumentService(MongoService mongo) : base(mongo) {}

        protected override IMongoCollection<Category> Items => MongoService.Categories;
    }
}