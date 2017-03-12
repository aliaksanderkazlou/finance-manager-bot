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
            return collection;
        }

        private static void AddCommandHandlerServices(this IServiceCollection collection)
        {
            collection.AddTransient<ICommandHandlerService, InlineCommandHandlerService>();
            collection.AddTransient<ICommandHandlerService, HelpCommandHandlerService>();
        }
    }
}