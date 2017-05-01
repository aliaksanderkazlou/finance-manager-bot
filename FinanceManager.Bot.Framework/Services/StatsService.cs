using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.DataAccessLayer.Services.Categories;
using FinanceManager.Bot.DataAccessLayer.Services.Operations;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;
using FinanceManager.Bot.Helpers.Enums;

namespace FinanceManager.Bot.Framework.Services
{
    public class StatsService
    {
        private readonly ICategoryDocumentService _categoryDocumentService;
        private readonly IOperationDocumentService _operationDocumentService;

        public StatsService(
            ICategoryDocumentService categoryDocumentService,
            IOperationDocumentService operationDocumentService)
        {
            _categoryDocumentService = categoryDocumentService;
            _operationDocumentService = operationDocumentService;
        }

        public async Task<List<HandlerServiceResult>> BuildStatistics(Category category, DateTime? from = null, DateTime? to = null)
        {
            var result = new List<HandlerServiceResult>
            {
                new HandlerServiceResult
                {
                    Message = "Here's your statistics:",
                    StatusCode = StatusCodeEnum.Ok
                }
            };

            var operations = await _operationDocumentService.GetByCategoryId(category.Id);

            if (from != null && to != null)
            {
                operations = operations.Where(o => o.Date >= from && o.Date <= to).ToList();
            }

            operations.Sort((x, y) => x.Date.CompareTo(y.Date));

            var sign = category.Type == CategoryTypeEnum.Income ? "+" : "-";

            result.AddRange(operations.Select(operation => new HandlerServiceResult
            {
                Message = $"{operation.Date}: {sign}{BuildAmountWithCurrency((float) operation.CreditAmountInCents / 100, category.Currency)}",
                StatusCode = StatusCodeEnum.Ok
            }));

            return result;
        }

        public async Task<List<HandlerServiceResult>> BuildStatistics(User user, CategoryTypeEnum type = CategoryTypeEnum.None)
        {
            var result = new List<HandlerServiceResult>
            {
                new HandlerServiceResult
                {
                    Message = "Here's your statistics:",
                    StatusCode = StatusCodeEnum.Ok
                }
            };

            var categories = await _categoryDocumentService.GetByUserIdAsync(user.Id);

            if (type != CategoryTypeEnum.None)
            {
                categories = categories.Where(c => c.Type == type && c.Configured).ToList();
            }

            foreach (var category in categories)
            {
                if (category.Type == CategoryTypeEnum.Income)
                {
                    result.Add(new HandlerServiceResult
                    {
                        Message = $"{category.Name}: +{BuildAmountWithCurrency((float) category.IncomeForThisMonthInCents / 100, category.Currency)} this month; " +
                                  $"+{BuildAmountWithCurrency((float) category.IncomeInCents / 100, category.Currency)} for all time.\n",
                        StatusCode = StatusCodeEnum.Ok
                    });
                }
                else
                {
                    result.Add(new HandlerServiceResult
                    {
                        Message = $"{category.Name}: -{BuildAmountWithCurrency((float)category.ExpenseForThisMonthInCents / 100, category.Currency)} this month; " +
                                  $"-{BuildAmountWithCurrency((float)category.ExpenseInCents / 100, category.Currency)} for all time.\n",
                        StatusCode = StatusCodeEnum.Ok
                    });
                }
            }

            return result;
        }

        private string BuildAmountWithCurrency(float amount, string currency)
        {
            if (currency.Equals("USD"))
            {
                return $"${amount}";
            }
            if (currency.Equals("EUR"))
            {
                return $"{amount}€";
            }

            return $"{amount}р.";
        }
    }
}