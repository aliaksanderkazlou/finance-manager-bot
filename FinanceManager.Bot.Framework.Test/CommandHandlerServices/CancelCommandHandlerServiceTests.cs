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
using Moq;
using NUnit.Framework;

namespace FinanceManager.Bot.Framework.Test.CommandHandlerServices
{
    [TestFixture]
    public class CancelCommandHandlerServiceTests
    {
        private Mock<IUserDocumentService> userDocumentService;
        private Mock<ICategoryDocumentService> categoryDocumentService;
        private Mock<IOperationDocumentService> operationDocumentService;
        
        [SetUp]
        public void SetUp()
        {
            userDocumentService = new Mock<IUserDocumentService>();
            categoryDocumentService = new Mock<ICategoryDocumentService>();
            operationDocumentService = new Mock<IOperationDocumentService>();
        }
        
        [Test]
        public async Task Handle_WhenCategoryIdAndOperationIdAreNull_ShouldRefreshUserAndReturnOkResult()
        {
            // Arrange
            const int chatId = 1;
            var id = Guid.NewGuid().ToString();

            var user = TestsHelper.GetUser(id: id, chatId: chatId);

            var message = TestsHelper.GetMessage(chatId: chatId);           

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

            userDocumentService.Verify(
                m => m.UpdateAsync(It.Is<User>(u =>
                    u.Id.Equals(id) && u.Context.CurrentNode == null && u.Context.OperationId == null &&
                    u.Context.CategoryId == null)), Times.Once);
        }
        
        [Test]
        public async Task Handle_WhenCategoryIdIsSetAndCategoryNotConfiguredAndOperationIdIsNull_ShouldRemoveCategoryAndRefreshUserAndReturnOkResult()
        {
            // Arrange
            const int chatId = 1;
            var categoryId = Guid.NewGuid().ToString();
            var id = Guid.NewGuid().ToString();

            var user = TestsHelper.GetUser(id: id, chatId: chatId);

            user.Context.CategoryId = categoryId;

            var message = TestsHelper.GetMessage(chatId: chatId);

            var category = new Category
            {
                Configured = false
            };

            categoryDocumentService.Setup(m => m.GetByIdAsync(categoryId)).ReturnsAsync(category);
            categoryDocumentService.Setup(m => m.DeleteAsync(categoryId)).Returns(Task.CompletedTask);
            
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
            
            userDocumentService.Verify(
                m => m.UpdateAsync(It.Is<User>(u =>
                    u.Id.Equals(id) && u.Context.CurrentNode == null && u.Context.OperationId == null &&
                    u.Context.CategoryId == null)), Times.Once);
            
            categoryDocumentService.Verify(m => m.DeleteAsync(categoryId), Times.Once);
        }
    }
}