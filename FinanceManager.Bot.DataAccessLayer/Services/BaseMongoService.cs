using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FinanceManager.Bot.DataAccessLayer.Services
{
    public abstract class BaseMongoService<T> : IDocumentService<T> where T : BaseModel
    {
        protected abstract IMongoCollection<T> Items { get; }

        public virtual string GenerateNewId()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        public virtual async Task InsertAsync(T document)
        {
            if (string.IsNullOrEmpty(document.Id))
            {
                document.Id = GenerateNewId();
            }

            await Items.InsertOneAsync(document);
        }

        public virtual async Task InsertAsync(params T[] documents)
        {
            await Items.InsertManyAsync(documents);
        }

        public virtual async Task UpdateAsync(T document)
        {
            var filter = Builders<T>.Filter.Eq(f => f.Id, document.Id);

            await Items.ReplaceOneAsync(filter, document);
        }

        public virtual async Task DeleteAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq(f => f.Id, id);

            await Items.DeleteOneAsync(filter);
        }

        public virtual async Task<T> GetByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq(f => f.Id, id);

            return await Items.FindAsync(filter).Result.FirstAsync();
        }

        //public async Task<List<T>> SearchByFilter(FilterDefinition<T> filter)
        //{
        //    return await Items.FindAsync(filter);
        //}
    }
}