using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.DataAccessLayer.Services.Categories;
using FinanceManager.Bot.DataAccessLayer.Services.Operations;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.Framework.CommandHandlerServices;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;
using FinanceManager.Bot.Helpers.Models;
using Moq;
using NUnit.Framework;

namespace FinanceManager.Bot.Framework.Test.CommandHandlerServices
{
    [TestFixture]
    public class CancelCommandHandlerServiceTests
    {
        [Test]
        public async Task Handle_WhenCategoryIdAndOperationIdAreNull_ShouldRefreshUserAndReturnOkResult()
        {
            // Arrange
            const int chatId = 1;
            var id = Guid.NewGuid().ToString();
            
            var user = new User
            {
                ChatId = chatId,
                FirstName = "Alex",
                LastName = "Kozlov",
                Id = id
            };

            var message = new Message
            {
                UserInfo = new UserInfo
                {
                    ChatId = chatId
                }
            };
            
            var userDocumentService = new Mock<IUserDocumentService>();
            var categoryDocumentService = new Mock<ICategoryDocumentService>();
            var operationDocumentService = new Mock<IOperationDocumentService>();

            userDocumentService.Setup(m => m.GetByChatId(chatId)).ReturnsAsync(user);
            userDocumentService.Setup(m => m.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            var underTest = new CancelCommandHandlerService(
                userDocumentService.Object,
                categoryDocumentService.Object,
                operationDocumentService.Object);
            
            // Act
            var result = await underTest.Handle(message);
            
            // Assert
            Assert.IsInstanceOf<List<HandlerServiceResult>>(result);
            
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(StatusCodeEnum.Ok, result[0].StatusCode);
        }
    }
}