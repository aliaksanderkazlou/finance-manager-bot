using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.DataAccessLayer.Services.Categories;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Helpers;
using FinanceManager.Bot.Framework.Results;
using FinanceManager.Bot.Framework.Services;
using FinanceManager.Bot.Helpers.Enums;
using FinanceManager.Bot.Helpers.Structures;
using User = FinanceManager.Bot.DataAccessLayer.Models.User;
using Message = FinanceManager.Bot.Helpers.Models.Message;

namespace FinanceManager.Bot.Framework.CommandHandlerServices
{
    public class CategoryCommandHandlerService : ICommandHandlerService
    {
        private readonly IUserDocumentService _userDocumentService;
        private readonly ICategoryDocumentService _categoryDocumentService;
        private delegate Task<List<HandlerServiceResult>> QuestionsHandlerDelegate(string answer, User user);
        private Dictionary<QuestionsEnum, QuestionsHandlerDelegate> _questionsHandlerDictionary;
        private readonly QuestionService _questionService;
        private readonly ResultService _resultService;
        private QuestionTree _tree;

        public CategoryCommandHandlerService(
            IUserDocumentService userDocumentService,
            ICategoryDocumentService categoryDocumentService,
            QuestionService questionService,
            ResultService resultService)
        {
            _categoryDocumentService = categoryDocumentService;
            _userDocumentService = userDocumentService;
            _questionService = questionService;
            _resultService = resultService;
            InitializeQuestionsHandlerDictionary();
            InitializeQuestionTree();
        }

        private void InitializeQuestionTree()
        {
            _tree = new QuestionTree
            {
                Children = new List<QuestionTree>
                {
                    //delete
                    new QuestionTree
                    {
                        Question = QuestionsEnum.ChooseCategoryToDelete,
                        Children = new List<QuestionTree>()
                    },
                    //edit
                    new QuestionTree
                    {
                        Question = QuestionsEnum.ChooseCategoryToEdit,
                        Children = new List<QuestionTree>
                        {
                            new QuestionTree
                            {
                                Question = QuestionsEnum.CategoryName,
                                Children = new List<QuestionTree>
                                {
                                    new QuestionTree
                                    {
                                        Question = QuestionsEnum.CategoryCurrency,
                                        Children = new List<QuestionTree>
                                        {
                                            new QuestionTree
                                            {
                                                Question = QuestionsEnum.CategoryType,
                                                Children = new List<QuestionTree>
                                                {
                                                    new QuestionTree
                                                    {
                                                        Question = QuestionsEnum.CategorySupposedToSpentThisMonth,
                                                        Children = new List<QuestionTree>
                                                        {
                                                            new QuestionTree
                                                            {
                                                                Question = QuestionsEnum.None,
                                                                Children = new List<QuestionTree>()
                                                            }
                                                        }
                                                    },
                                                    new QuestionTree
                                                    {
                                                        Question = QuestionsEnum.None,
                                                        Children = new List<QuestionTree>()
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    //add
                    new QuestionTree
                    {
                        Question = QuestionsEnum.CategoryName,
                        Children = new List<QuestionTree>
                        {
                            new QuestionTree
                            {
                                Question = QuestionsEnum.CategoryCurrency,
                                Children = new List<QuestionTree>
                                {
                                    new QuestionTree
                                    {
                                        Question = QuestionsEnum.CategoryType,
                                        Children = new List<QuestionTree>
                                        {
                                            new QuestionTree
                                            {
                                                Question = QuestionsEnum.CategorySupposedToSpentThisMonth,
                                                Children = new List<QuestionTree>
                                                {
                                                    new QuestionTree
                                                    {
                                                        Question = QuestionsEnum.None,
                                                        Children = new List<QuestionTree>()
                                                    }
                                                }
                                            },
                                            new QuestionTree
                                            {
                                                Question = QuestionsEnum.None,
                                                Children = new List<QuestionTree>()
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                Question = QuestionsEnum.CategoryAction
            };
        }

        private void InitializeQuestionsHandlerDictionary()
        {
            _questionsHandlerDictionary = new Dictionary<QuestionsEnum, QuestionsHandlerDelegate>
            {
                {QuestionsEnum.CategoryAction, ConfigureCategoryAction },
                {QuestionsEnum.CategorySupposedToSpentThisMonth, ConfigureCategorySupposedToSpentThisMonth },
                {QuestionsEnum.CategoryType, ConfigureCategoryType },
                {QuestionsEnum.AddNewCategoryOrNot, ConfigureCategoryCreating },
                {QuestionsEnum.CategoryName, ConfigureCategoryName},
                {QuestionsEnum.ChooseCategoryToDelete, ConfigureCategoryDelete },
                {QuestionsEnum.ChooseCategoryToEdit, ConfigureCategoryEdit },
                {QuestionsEnum.CategoryCurrency, ConfigureCategoryCurrency }
            };
        }

        private async Task<List<HandlerServiceResult>> ConfigureCategoryCurrency(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer) || !answer.Equals("EUR") && !answer.Equals("USD") && !answer.Equals("BYN"))
            {
                return new List<HandlerServiceResult>
                {
                    _resultService.BuildCategoryInvalidCurrencyErrorResult()
                };
            }

            var category = await _categoryDocumentService.GetByIdAsync(user.Context.CategoryId);

            category.Currency = answer;

            await _categoryDocumentService.UpdateAsync(category);

            user.Context.CurrentNode = user.Context.CurrentNode.Children.FirstOrDefault();

            await _userDocumentService.UpdateAsync(user);

            return new List<HandlerServiceResult>
            {
                await _questionService.BuildQuestion(user)
            };
        }

        private async Task<List<HandlerServiceResult>> ConfigureCategoryEdit(string answer, User user)
        {
            answer = answer.Trim();

            List<HandlerServiceResult> result;

            var categories = await _categoryDocumentService.GetByUserIdAsync(user.Id);

            var categoryToEdit = categories.FirstOrDefault(c => c.Name.Equals(answer));

            if (categoryToEdit != null)
            {
                user.Context.CurrentNode = user.Context.CurrentNode.Children.FirstOrDefault();
                user.Context.CategoryId = categoryToEdit.Id;

                result = new List<HandlerServiceResult>
                {
                    await _questionService.BuildQuestion(user)
                };
            }
            else
            {
                user.Context.CurrentNode.Question = QuestionsEnum.None;

                user.Context.CategoryId = null;

                result = new List<HandlerServiceResult>
                {
                    _resultService.BuildCategoryNotFoundErrorResult()
                };
            }

            await _userDocumentService.UpdateAsync(user);

            return result;
        }

        private async Task<List<HandlerServiceResult>> ConfigureCategoryDelete(string answer, User user)
        {
            answer = answer.Trim();

            List<HandlerServiceResult> result;

            var categories = await _categoryDocumentService.GetByUserIdAsync(user.Id);

            var categoryToDelete = categories.FirstOrDefault(c => c.Name.Equals(answer));

            if (categoryToDelete != null)
            {
                await _categoryDocumentService.DeleteAsync(categoryToDelete.Id);

                result = new List<HandlerServiceResult> {_resultService.BuildCategoryDeletedResult()};
            }
            else
            {
                result = new List<HandlerServiceResult>{ _resultService.BuildCategoryNotFoundErrorResult()};
            }

            user.Context.CurrentNode = null;

            user.Context.CategoryId = null;

            await _userDocumentService.UpdateAsync(user);

            return result;
        }

        private async Task<List<HandlerServiceResult>> ConfigureCategoryName(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer))
            {
                return new List<HandlerServiceResult>
                {
                    _resultService.BuildClearCategoryNameErrorResult()
                };
            }

            if (answer.Length > 30)
            {
                return new List<HandlerServiceResult>
                {
                    _resultService.BuildLongCategoryNameErrorResult()
                };
            }

            var categoriesWithTheSameName = await _categoryDocumentService.GetByUserIdAsync(user.Id);

            categoriesWithTheSameName = categoriesWithTheSameName.Where(c =>c.Configured && c.Name.Equals(answer)).ToList();

            if (categoriesWithTheSameName.Count > 0)
            {
                return new List<HandlerServiceResult>
                {
                    _resultService.BuildNotUniqueCategoryNameErrorResult()
                };
            }

            var category = await _categoryDocumentService.GetByIdAsync(user.Context.CategoryId);

            category.Name = answer;

            await _categoryDocumentService.UpdateAsync(category);

            user.Context.CurrentNode = user.Context.CurrentNode.Children.FirstOrDefault();

            await _userDocumentService.UpdateAsync(user);

            return new List<HandlerServiceResult>
            {
                await _questionService.BuildQuestion(user)
            };
        }

        private async Task<List<HandlerServiceResult>> ConfigureCategoryCreating(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer) || !answer.Contains("Yes") && !answer.Contains("No"))
            {
                return new List<HandlerServiceResult>
                {
                    _resultService.BuildYouShouldTypeOnlyYesOrNoErrorResult()
                };
            }

            if (answer.Equals("Yes"))
            {
                var category = new Category
                {
                    Id = _categoryDocumentService.GenerateNewId(),
                    UserId = user.Id,
                    Configured = false
                };

                await _categoryDocumentService.InsertAsync(category);

                user.Context.CurrentNode = user.Context.CurrentNode.Children[0].Children[2];
                user.Context.CategoryId = category.Id;

                await _userDocumentService.UpdateAsync(user);

                return new List<HandlerServiceResult>
                {
                    await _questionService.BuildQuestion(user)
                };
            }

            return new List<HandlerServiceResult>
            {
                _resultService.BuildCategoryActionsResult()
            };
        }

        private async Task<List<HandlerServiceResult>> ConfigureCategoryType(string answer, User user)
        {
            answer = answer.Trim();

            List<HandlerServiceResult> result;

            if (string.IsNullOrEmpty(answer) || !answer.Contains("Income") && !answer.Contains("Expense"))
            {
                return new List<HandlerServiceResult>
                {
                    _resultService.BuildCategoryInvalidTypeErrorResult()
                };
            }

            var categoryType = answer.Equals("Income") ? CategoryTypeEnum.Income : CategoryTypeEnum.Expense;

            var category = await _categoryDocumentService.GetByIdAsync(user.Context.CategoryId);

            category.Type = categoryType;

            if (categoryType == CategoryTypeEnum.Income)
            {
                category.Configured = true;
                user.Context.CategoryId = null;
                user.Context.CurrentNode = null;

                result = new List<HandlerServiceResult>
                {
                    _resultService.BuildFinishedConfiguringCategoryResult()
                };
            }
            else
            {
                user.Context.CurrentNode =
                    user.Context.CurrentNode.FindChildByQuestion(QuestionsEnum.CategorySupposedToSpentThisMonth);

                result = new List<HandlerServiceResult>
                {
                    await _questionService.BuildQuestion(user)
                };
            }

            await _categoryDocumentService.UpdateAsync(category);
            await _userDocumentService.UpdateAsync(user);

            return result;
        }

        private async Task<List<HandlerServiceResult>> ConfigureCategoryAction(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer) || !answer.Contains("Add new category") && !answer.Contains("Edit category") && !answer.Contains("Delete category"))
            {
                return new List<HandlerServiceResult>
                {
                    _resultService.BuildCategoryActionWrongAnswerErrorResult()
                };
            }

            if (answer.Equals("Add new category"))
            {
                var category = new Category
                {
                    Id = _categoryDocumentService.GenerateNewId(),
                    UserId = user.Id,
                    Configured = false
                };

                await _categoryDocumentService.InsertAsync(category);

                user.Context.CategoryId = category.Id;

                user.Context.CurrentNode = user.Context.CurrentNode.FindChildByQuestion(QuestionsEnum.CategoryName);
            }
            else if (answer.Equals("Edit category"))
            {
                user.Context.CurrentNode = user.Context.CurrentNode.FindChildByQuestion(QuestionsEnum.ChooseCategoryToEdit);
            }
            else
            {
                user.Context.CurrentNode = user.Context.CurrentNode.FindChildByQuestion(QuestionsEnum.ChooseCategoryToDelete);
            }

            await _userDocumentService.UpdateAsync(user);

            return new List<HandlerServiceResult>
            {
                await _questionService.BuildQuestion(user)
            };
        }

        private async Task<List<HandlerServiceResult>> ConfigureCategorySupposedToSpentThisMonth(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer) || !long.TryParse(answer, out long number) || number <= 0)
            {
                return new List<HandlerServiceResult>
                {
                    _resultService.BuildCategoryInvalidSupposedToSpent()
                };
            }

            var category = await _categoryDocumentService.GetByIdAsync(user.Context.CategoryId);

            category.SupposedToSpentThisMonthInCents = number * 100;
            category.ExpenseForThisMonthInCents = 0;
            category.ExpenseInCents = 0;
            category.Configured = true;

            await _categoryDocumentService.UpdateAsync(category);

            user.Context.CurrentNode = null;
            user.Context.CategoryId = null;

            await _userDocumentService.UpdateAsync(user);

            return new List<HandlerServiceResult>
            {
                _resultService.BuildFinishedConfiguringCategoryResult()
            };
        }

        public async Task<List<HandlerServiceResult>> HandleCategoryQuestion(
            string answer,
            User user)
        {
            List<HandlerServiceResult> result;

            try
            {
                result = await _questionsHandlerDictionary[user.Context.CurrentNode.Question].Invoke(answer, user);
            }
            catch (KeyNotFoundException)
            {
                result = new List<HandlerServiceResult>
                {
                    new HandlerServiceResult
                    {
                        StatusCode = StatusCodeEnum.Bad,
                        Message = "Sorry, something went wrong. Please, try /cancel the command and start again."
                    }
                };
            }

            return result;
        }

        public async Task<List<HandlerServiceResult>> Handle(Message message)
        {
            var user = await _userDocumentService.GetByChatId(message.UserInfo.ChatId);

            var categories = await _categoryDocumentService.GetByUserIdAsync(user.Id);

            categories = categories.Where(c => c.Configured).ToList();

            if (categories.Count > 0)
            {
                user.Context = new Context
                {
                    CurrentNode = _tree
                };
            }
            else
            {
                var parentTree = new QuestionTree
                {
                    Children = new List<QuestionTree>
                    {
                        _tree
                    },
                    Question = QuestionsEnum.AddNewCategoryOrNot
                };

                user.Context = new Context()
                {
                    CurrentNode = parentTree
                };
            }

            await _userDocumentService.UpdateAsync(user);

            return new List<HandlerServiceResult>
            {
                await _questionService.BuildQuestion(user)
            };
        }
    }
}