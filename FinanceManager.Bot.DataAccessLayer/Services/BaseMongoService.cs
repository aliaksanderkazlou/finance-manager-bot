using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FinanceManager.Bot.DataAccessLayer.Services
{
    public abstract class BaseMongoService<T>
    {
        protected abstract IMongoCollection<T> Items { get; }

        public virtual string GenerateNewId()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        public virtual async Task InsertAsync(T document)
        {
            await Items.InsertOneAsync(document);
        }

        public virtual async Task InsertAsync(params T[] documents)
        {
            await Items.InsertManyAsync(documents);
        }
    }
}