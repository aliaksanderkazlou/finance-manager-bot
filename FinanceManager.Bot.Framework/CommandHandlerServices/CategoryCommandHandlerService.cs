using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.DataAccessLayer.Services.Categories;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;
using FinanceManager.Bot.Helpers.Enums;
using Telegram.Bot.Types;
using User = FinanceManager.Bot.DataAccessLayer.Models.User;

namespace FinanceManager.Bot.Framework.CommandHandlerServices
{
    public class CategoryCommandHandlerService : ICommandHandlerService
    {
        private readonly IUserDocumentService _userDocumentService;
        private readonly ICategoryDocumentService _categoryDocumentService;
        private delegate Task<HandlerServiceResult> QuestionsHandlerDelegate(string answer, User user);
        private Dictionary<QuestionsEnum, QuestionsHandlerDelegate> _questionsHandlerDictionary;

        public CategoryCommandHandlerService(
            IUserDocumentService userDocumentService,
            ICategoryDocumentService categoryDocumentService)
        {
            _categoryDocumentService = categoryDocumentService;
            _userDocumentService = userDocumentService;
            InitializeQuestionsHandlerDictionary();
        }

        private void InitializeQuestionsHandlerDictionary()
        {
            _questionsHandlerDictionary = new Dictionary<QuestionsEnum, QuestionsHandlerDelegate>
            {
                {QuestionsEnum.CategoryCurrency, ConfigureCategoryCurrency },
                {QuestionsEnum.CategoryOperation, ConfigureCategoryOperation },
                {QuestionsEnum.CategorySupposedToSpentThisMonth, ConfigureCategorySupposedToSpentThisMonth },
                {QuestionsEnum.CategoryType, ConfigureCategoryType }
            };
        }

        private async Task<HandlerServiceResult> ConfigureCategoryType(string answer, User user)
        {
            return new HandlerServiceResult();
        }

        private async Task<HandlerServiceResult> ConfigureCategoryCurrency(string answer, User user)
        {
            return new HandlerServiceResult();
        }

        private async Task<HandlerServiceResult> ConfigureCategoryOperation(string answer, User user)
        {
            return new HandlerServiceResult();
        }

        private async Task<HandlerServiceResult> ConfigureCategorySupposedToSpentThisMonth(string answer, User user)
        {
            return new HandlerServiceResult();
        }

        public async Task<HandlerServiceResult> HandleCategoryQuestion(
            string answer,
            User user)
        {
            HandlerServiceResult result;

            try
            {
                result = await _questionsHandlerDictionary[user.Context.LastQuestion].Invoke(answer, user);
            }
            catch (KeyNotFoundException)
            {
                result = new HandlerServiceResult
                {
                    StatusCode = StatusCodeEnum.Bad,
                    Message = ""
                };
            }

            return result;
        }

        public async Task<HandlerServiceResult> Handle(Message message)
        {
            //var words = message.Text.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);

            //if (words.Length == 1)
            //{
            //    await _userDocumentService.InsertAsync(new User
            //    {
            //        Id = message.Chat.Id.ToString(),
            //        Categories = new List<Category>()
            //    });
            //}

            return new HandlerServiceResult
            {
                StatusCode = StatusCodeEnum.Ok,
                Message = "You don't have any categories yet. Do you want to add categories?"
            };
        }

        //private HandlerServiceResult 
    }
}