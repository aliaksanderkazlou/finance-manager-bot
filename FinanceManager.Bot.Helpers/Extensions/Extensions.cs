using System;

namespace FinanceManager.Bot.Helpers.Extensions
{
    public static class Extensions
    {
        public static TOut With<TIn, TOut>(this TIn self, Func<TIn, TOut> f)
            where TIn : class 
            where TOut : class
        {
            return self == null ? null : f(self);
        }
    }
}