using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using MongoDB.Driver;

namespace FinanceManager.Bot.DataAccessLayer.Services.UserStatuses
{
    public class UserStatusDocumentService : BaseDocumentService<UserStatus>, IUserStatusDocumentService
    {
        public UserStatusDocumentService(MongoService mongo) : base(mongo)
        {
        }

        protected override IMongoCollection<UserStatus> Items => MongoService.UserStatuses;

        public async Task<UserStatus> GetByUserId(string userId)
        {
            var filter = Builders<UserStatus>.Filter.Eq(f => f.UserId, userId);

            var searchResult = await SearchByFilter(filter);

            return searchResult.FirstOrDefault();
        }
    }
}