using MongoDB.Bson;
using MongoDB.Driver;

namespace FinanceManager.Bot.DataAccessLayer.Services
{
    public abstract class BaseMongoService<T>
    {
        protected abstract IMongoCollection<BsonDocument> Items { get; }

        public virtual string GenerateNewId()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        public virtual void Insert(T document)
        {
            //Items.InsertOneAsync(new BsonDocument(document));
        }
    }
}