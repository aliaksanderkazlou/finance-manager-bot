using FinanceManager.Bot.DataAccessLayer;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.Framework;
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
            collection.AddTransient<InlineCommandHandlerService>();
            collection.AddTransient<HelpCommandHandlerService>();
            collection.AddTransient<CategoryCommandHandlerService>();
        }

        private static void AddDocumentServices(this IServiceCollection collection)
        {
            collection.AddTransient<IUserDocumentService, UserDocumentService>();
            collection.AddTransient<MongoService>();
        }
    }
}