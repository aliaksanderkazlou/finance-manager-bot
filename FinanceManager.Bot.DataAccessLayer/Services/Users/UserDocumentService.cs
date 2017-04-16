using System;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.DataAccessLayer.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FinanceManager.Bot.DataAccessLayer.Services.Users
{
    public class UserDocumentService: BaseDocumentService<User>, IUserDocumentService
    {
        protected override IMongoCollection<BsonDocument> Items => MongoService.Users;

        public UserDocumentService(MongoService mongo) : base(mongo)
        {
            
        }

        public void Create(User item)
        {
            throw new System.NotImplementedException();
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