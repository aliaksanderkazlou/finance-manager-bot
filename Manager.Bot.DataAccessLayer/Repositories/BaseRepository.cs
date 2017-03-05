using MongoDB.Bson;
using MongoDB.Driver;

namespace Manager.Bot.DataAccessLayer.Repositories
{
    public abstract class BaseRepository <T> 
        where T: class
    {
        protected abstract IMongoCollection<T> Items { get; }

        public virtual string GenerateNewId()
        {
            return ObjectId.GenerateNewId().ToString();
        }
    }
}