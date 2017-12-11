using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using MongoDB.Driver;

namespace FinanceManager.Bot.DataAccessLayer.Services.Chats
{
    public class ChatDocumentService : BaseDocumentService<Chat>, IChatDocumentService
    {
        public ChatDocumentService(MongoService mongo) : base(mongo)
        {
        }

        protected override IMongoCollection<Chat> Items => MongoService.Chats;
    }
}