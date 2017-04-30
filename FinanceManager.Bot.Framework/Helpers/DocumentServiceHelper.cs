using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.DataAccessLayer.Services.Categories;
using FinanceManager.Bot.DataAccessLayer.Services.Operations;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.Helpers.Enums;

namespace FinanceManager.Bot.Framework.Helpers
{
    public class DocumentServiceHelper
    {
        private readonly IUserDocumentService _userDocumentService;
        private readonly ICategoryDocumentService _categoryDocumentService;
        private readonly IOperationDocumentService _operationDocumentService;

        public DocumentServiceHelper(
            IUserDocumentService userDocumentService,
            ICategoryDocumentService categoryDocumentService,
            IOperationDocumentService operationDocumentService)
        {
            _userDocumentService = userDocumentService;
            _categoryDocumentService = categoryDocumentService;
            _operationDocumentService = operationDocumentService;
        }

        public async Task InsertOperationAsync(Operation operation)
        {
            await _operationDocumentService.InsertAsync(operation);
        }

        public async Task UpdateOperationAsync(Operation operation)
        {
            await _operationDocumentService.UpdateAsync(operation);
        }

        public async Task<User> GetUserByChatIdAsync(long chatId)
        {
            return await _userDocumentService.GetByChatId(chatId);
        }

        public async Task<List<Category>> GetUserCategories(string userId)
        {
            return await _categoryDocumentService.GetByUserIdAsync(userId);
        }

        public async Task DeleteUserContextAsync(User user)
        {
           // user.Context.LastQuestion = QuestionsEnum.None;
            user.Context.OperationId = null;
            user.Context.CategoryId = null;
            //user.Context.Questions = null;

            await _userDocumentService.UpdateAsync(user);
        }

        public async Task<List<Category>> GetUserCategoriesByTypeAsync(User user)
        {
            var userCategories = await _categoryDocumentService.GetByUserIdAsync(user.Id);

            return userCategories.Where(c => c.Type == user.Context.CategoryType).ToList();
        }

        public async Task<Operation> GetOperationByIdAsync(string id)
        {
            return await _operationDocumentService.GetByIdAsync(id);
        }
    }
}