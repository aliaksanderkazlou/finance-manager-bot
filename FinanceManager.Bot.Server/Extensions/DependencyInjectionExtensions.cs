using FinanceManager.Bot.DataAccessLayer;
using FinanceManager.Bot.DataAccessLayer.Services.Categories;
using FinanceManager.Bot.DataAccessLayer.Services.UnhandledMessages;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.Framework.CommandHandlerServices;
using FinanceManager.Bot.Framework.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceManager.Bot.Server.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddAppDependencies(this IServiceCollection collection)
        {
            collection.AddTransient<CommandService>();
            collection.AddCommandHandlerServices();
            collection.AddDocumentServices();
            return collection;
        }

        private static void AddCommandHandlerServices(this IServiceCollection collection)
        {
            collection.AddTransient<HelpCommandHandlerService>();
            collection.AddTransient<CategoryCommandHandlerService>();
            collection.AddTransient<UnhandledMessageService>();
            collection.AddTransient<CancelCommandHandlerService>();
        }

        private static void AddDocumentServices(this IServiceCollection collection)
        {
            collection.AddTransient<IUserDocumentService, UserDocumentService>();
            collection.AddTransient<ICategoryDocumentService, CategoryDocumentService>();
            collection.AddTransient<IUnhandledMessageDocumentService, UnhandledMessageDocumentService>();

            collection.AddTransient<MongoService>();
        }
    }
}