using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FinanceManager.Bot.DataAccessLayer.Services.Users
{
    public class UserDocumentService: BaseDocumentService<User>, IUserDocumentService
    {
        protected override IMongoCollection<User> Items => MongoService.Users;

        public UserDocumentService(MongoService mongo) : base(mongo) {}

        public async Task CreateAsync(User item)
        {
            if (string.IsNullOrEmpty(item.Id))
            {
                item.Id = GenerateNewId();
            }

            await InsertAsync(item);
        }

        public void Update(User item)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public User GetById(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}