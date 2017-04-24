using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FinanceManager.Bot.DataAccessLayer.Services.Users
{
    public class UserDocumentService: BaseDocumentService<User>, IUserDocumentService
    {
        public UserDocumentService(MongoService mongo) : base(mongo) {}

        protected override IMongoCollection<User> Items => MongoService.Users;

        public async Task<User> GetByChatId(long chatId)
        {
            var filter = Builders<User>.Filter.Eq(f => f.ChatId, chatId);

            var searchResult = await SearchByFilter(filter);

            return searchResult.FirstOrDefault();
        }
    }
}