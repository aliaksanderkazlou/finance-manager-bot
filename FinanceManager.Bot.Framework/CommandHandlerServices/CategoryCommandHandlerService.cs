using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManager.Bot.DataAccessLayer.Models;
using FinanceManager.Bot.DataAccessLayer.Services.Categories;
using FinanceManager.Bot.DataAccessLayer.Services.Users;
using FinanceManager.Bot.Framework.Enums;
using FinanceManager.Bot.Framework.Results;
using FinanceManager.Bot.Framework.Structures;
using FinanceManager.Bot.Helpers.Enums;
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
        private QuestionTree _tree;

        public CategoryCommandHandlerService(
            IUserDocumentService userDocumentService,
            ICategoryDocumentService categoryDocumentService)
        {
            _categoryDocumentService = categoryDocumentService;
            _userDocumentService = userDocumentService;
            InitializeQuestionsHandlerDictionary();
            InitializeQuestionTree();
        }

        private void InitializeQuestionTree()
        {
            _tree = new QuestionTree
            {
                Parent = null,
                Question = QuestionsEnum.CategoryAction
            };

            _tree.Children = new List<QuestionTree>();

            // delete node
            _tree.Children.Add(new QuestionTree
            {
                Parent = _tree,
                Question = QuestionsEnum.DeleteCategory
            });

            _tree = _tree.Children[0];

            _tree.Children = new List<QuestionTree>();

            _tree.Children.Add(new QuestionTree
            {
                Parent = _tree,
                Question = QuestionsEnum.ChooseCategory
            });

            while (_tree.Parent != null)
            {
                _tree = _tree.Parent;
            }

            // edit node
            _tree.Children.Add(new QuestionTree
            {
                Parent = _tree,
                Question = QuestionsEnum.EditCategory
            });

            _tree = _tree.Children[1];

            _tree.Children = new List<QuestionTree>();

            _tree.Children.Add(new QuestionTree
            {
                Parent = _tree,
                Question = QuestionsEnum.ChooseCategory
            });

            while (_tree.Parent != null)
            {
                _tree = _tree.Parent;
            }

            // add node
            _tree.Children.Add(new QuestionTree
            {
                Parent = _tree,
                Question = QuestionsEnum.AddCategory
            });

            _tree = _tree.Children[2];

            _tree.Children = new List<QuestionTree>();

            _tree.Children.Add(new QuestionTree
            {
                Parent = _tree,
                Question = QuestionsEnum.CategoryName
            });

            _tree = _tree.Children[0];

            _tree.Children = new List<QuestionTree>();

            _tree.Children.Add(new QuestionTree
            {
                Parent = _tree,
                Question = QuestionsEnum.CategoryType
            });

            _tree = _tree.Children[0];

            _tree.Children = new List<QuestionTree>();

            _tree.Children.Add(new QuestionTree
            {
                Parent = _tree,
                Question = QuestionsEnum.CategorySupposedToSpentThisMonth
            });

            _tree.Children.Add(new QuestionTree
            {
                Parent = _tree,
                Question = QuestionsEnum.None
            });

            _tree = _tree.Children[0];

            _tree.Children = new List<QuestionTree>();

            _tree.Children.Add(new QuestionTree
            {
                Parent = _tree,
                Question = QuestionsEnum.None
            });

            while (_tree.Parent != null)
            {
                _tree = _tree.Parent;
            }
        }

        private void InitializeQuestionsHandlerDictionary()
        {
            _questionsHandlerDictionary = new Dictionary<QuestionsEnum, QuestionsHandlerDelegate>
            {
                {QuestionsEnum.CategoryAction, ConfigureCategoryOperation },
                {QuestionsEnum.CategorySupposedToSpentThisMonth, ConfigureCategorySupposedToSpentThisMonth },
                {QuestionsEnum.CategoryType, ConfigureCategoryType },
                {QuestionsEnum.AddNewCategoryOrNot, ConfigureCategoryCreating },
                {QuestionsEnum.CategoryName, ConfigureCategoryName},
                {QuestionsEnum.DeleteCategory, ConfigureCategoryDelete },
                {QuestionsEnum.EditCategory, ConfigureCategoryEdit }
            };
        }

        private async Task<List<HandlerServiceResult>> ConfigureCategoryEdit(string answer, User user)
        {
            answer = answer.Trim();

            var categories = await _categoryDocumentService.GetByUserIdAsync(user.Id);

            var categoryToEdit = categories.FirstOrDefault(c => c.Name.Equals(answer));

            if (categoryToEdit != null)
            {
                user.Context.LastQuestion = QuestionsEnum.CategoryName;

                user.Context.CategoryId = categoryToEdit.Id;

                await _userDocumentService.UpdateAsync(user);

                return new List<HandlerServiceResult>
                {
                    new HandlerServiceResult
                    {
                        Message = "Great! Please, type name of your category.",
                        StatusCode = StatusCodeEnum.Ok
                    }
                };
            }

            user.Context.LastQuestion = QuestionsEnum.None;

            user.Context.CategoryId = null;

            await _userDocumentService.UpdateAsync(user);

            return new List<HandlerServiceResult>
            {
                new HandlerServiceResult
                {
                    Message = "Sorry, you don't have any category with this name.",
                    StatusCode = StatusCodeEnum.Ok
                }
            };
        }

        private async Task<List<HandlerServiceResult>> ConfigureCategoryDelete(string answer, User user)
        {
            answer = answer.Trim();

            var categories = await _categoryDocumentService.GetByUserIdAsync(user.Id);

            var categoryToDelete = categories.FirstOrDefault(c => c.Name.Equals(answer));

            if (categoryToDelete != null)
            {
                user.Context.LastQuestion = QuestionsEnum.None;

                user.Context.CategoryId = null;

                await _userDocumentService.UpdateAsync(user);

                await _categoryDocumentService.DeleteAsync(categoryToDelete.Id);

                return new List<HandlerServiceResult>
                {
                    new HandlerServiceResult
                    {
                        Message = string.Format("Category {0} was deleted successfully.", categoryToDelete.Name),
                        StatusCode = StatusCodeEnum.Ok
                    }
                };
            }

            user.Context.LastQuestion = QuestionsEnum.None;

            user.Context.CategoryId = null;

            await _userDocumentService.UpdateAsync(user);

            return new List<HandlerServiceResult>
            {
                new HandlerServiceResult
                {
                    Message = "Sorry, you don't have any category with this name.",
                    StatusCode = StatusCodeEnum.Ok
                }
            };
        }

        private async Task<List<HandlerServiceResult>> ConfigureCategoryName(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer))
            {
                return new List<HandlerServiceResult>
                {
                    new HandlerServiceResult
                    {
                        Message = "Sorry, you should type something",
                        StatusCode = StatusCodeEnum.Bad
                    }
                };
            }

            if (answer.Length > 30)
            {
                return new List<HandlerServiceResult>
                {
                    new HandlerServiceResult
                    {
                        Message = "Sorry, you cannot type more than 30 symbols",
                        StatusCode = StatusCodeEnum.Bad
                    }
                };
            }

            var categoriesWithTheSameName = await _categoryDocumentService.GetByNameAsync(answer);

            if (categoriesWithTheSameName.Count > 0)
            {
                return new List<HandlerServiceResult>
                {
                    new HandlerServiceResult
                    {
                        Message = "Sorry, name should be unique.",
                        StatusCode = StatusCodeEnum.Bad
                    }
                };
            }

            var category = await _categoryDocumentService.GetByIdAsync(user.Context.CategoryId);

            category.Name = answer;

            await _categoryDocumentService.UpdateAsync(category);

            user.Context.LastQuestion = QuestionsEnum.CategoryType;

            await _userDocumentService.UpdateAsync(user);

            return new List<HandlerServiceResult>
            {
                new HandlerServiceResult
                {
                    StatusCode = StatusCodeEnum.NeedKeyboard,
                    Helper = new List<string>
                    {
                        "Income",
                        "Expense"
                    },
                    Message = "Now select the type of your category."
                }
            };
        }

        private async Task<List<HandlerServiceResult>> ConfigureCategoryCreating(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer) || !answer.Contains("Yes") && !answer.Contains("No"))
            {
                return new List<HandlerServiceResult>
                {
                    new HandlerServiceResult
                    {
                        Message = "Sorry, you can type only Yes or No, or you can /cancel command.",
                        Helper = new List<string>
                        {
                            "Yes",
                            "No"
                        },
                        StatusCode = StatusCodeEnum.NeedKeyboard
                    }
                };
            }

            if (answer.Equals("Yes"))
            {
                var category = new Category
                {
                    Id = _categoryDocumentService.GenerateNewId(),
                    UserId = user.Id,
                };

                await _categoryDocumentService.InsertAsync(category);

                user.Context.LastQuestion = QuestionsEnum.CategoryName;
                user.Context.CategoryId = category.Id;

                await _userDocumentService.UpdateAsync(user);

                return new List<HandlerServiceResult>
                {
                    new HandlerServiceResult
                    {
                        Message = "Great! Please, type name of your category.",
                        StatusCode = StatusCodeEnum.Ok
                    }
                };
            }

            return new List<HandlerServiceResult>
            {
                new HandlerServiceResult
                {
                    Message = "You always can create, edit or delete categories by /category command",
                    StatusCode = StatusCodeEnum.Ok
                }
            };
        }

        private async Task<List<HandlerServiceResult>> ConfigureCategoryType(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer) || !answer.Contains("Income") && !answer.Contains("Expense"))
            {
                return new List<HandlerServiceResult>
                {
                    new HandlerServiceResult
                    {
                        Message = "Sorry, you can type only Income or Expense, or you can /cancel command",
                        Helper = new List<string>
                        {
                            "Income",
                            "Expense"
                        },
                        StatusCode = StatusCodeEnum.NeedKeyboard
                    }
                };
            }

            var categoryType = answer.Equals("Income") ? CategoryTypeEnum.Income : CategoryTypeEnum.Expense;

            var category = await _categoryDocumentService.GetByIdAsync(user.Context.CategoryId);

            category.Type = categoryType;

            await _categoryDocumentService.UpdateAsync(category);

            user.Context.LastQuestion = categoryType == CategoryTypeEnum.Expense
                ? QuestionsEnum.CategorySupposedToSpentThisMonth
                : QuestionsEnum.None;

            
            if (categoryType == CategoryTypeEnum.Income)
            {
                user.Context.CategoryId = null;

                return new List<HandlerServiceResult>
                {
                    new HandlerServiceResult
                    {
                        Message = "Your category configured. Now you can create operations in this category by using /category command.",
                        StatusCode = StatusCodeEnum.Ok
                    }
                };
            }

            await _userDocumentService.UpdateAsync(user);

            return new List<HandlerServiceResult>
            {
                new HandlerServiceResult
                {
                    Message = string.Format("You choose {0}. Please, type the amount of money you would like to spend per month.", answer)
                }
            };
        }

        private async Task<List<HandlerServiceResult>> ConfigureCategoryOperation(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer) || !answer.Contains("Add new category") && !answer.Contains("Edit category") && !answer.Contains("Delete category"))
            {
                return new List<HandlerServiceResult>
                {
                    new HandlerServiceResult
                    {
                        Helper = new List<string>
                        {
                            "Add new category",
                            "Edit category",
                            "Delete category"
                        },
                        Message = "Sorry, you should chose one of three options, or you can /cancel command.",
                        StatusCode = StatusCodeEnum.NeedKeyboard
                    }
                };
            }

            if (answer.Equals("Add new category"))
            {
                var category = new Category
                {
                    Id = _categoryDocumentService.GenerateNewId(),
                    UserId = user.Id
                };

                await _categoryDocumentService.InsertAsync(category);

                user.Context.LastQuestion = QuestionsEnum.CategoryName;
                user.Context.CategoryId = category.Id;

                await _userDocumentService.UpdateAsync(user);

                return new List<HandlerServiceResult>
                {
                    new HandlerServiceResult
                    {
                        Message = "Great! Please, type name of your new category.",
                        StatusCode = StatusCodeEnum.Ok
                    }
                };
            }

            if (answer.Equals("Edit category"))
            {
                user.Context.LastQuestion = QuestionsEnum.EditCategory;

                await _userDocumentService.UpdateAsync(user);

                var categories = await _categoryDocumentService.GetByUserIdAsync(user.Id);

                return new List<HandlerServiceResult>
                {
                    new HandlerServiceResult
                    {
                        Message = "Please, choose category to edit.",
                        StatusCode = StatusCodeEnum.NeedKeyboard,
                        Helper = categories.Select(c => c.Name).ToList()
                    }
                };
            }
            else
            {
                user.Context.LastQuestion = QuestionsEnum.DeleteCategory;

                await _userDocumentService.UpdateAsync(user);

                var categories = await _categoryDocumentService.GetByUserIdAsync(user.Id);

                return new List<HandlerServiceResult>
                {
                    new HandlerServiceResult
                    {
                        Message = "Please, choose category to delete.",
                        StatusCode = StatusCodeEnum.NeedKeyboard,
                        Helper = categories.Select(c => c.Name).ToList()
                    }
                };
            }
        }

        private async Task<List<HandlerServiceResult>> ConfigureCategorySupposedToSpentThisMonth(string answer, User user)
        {
            answer = answer.Trim();

            if (string.IsNullOrEmpty(answer) || !long.TryParse(answer, out long number) || number <= 0)
            {
                return new List<HandlerServiceResult>
                {
                    new HandlerServiceResult
                    {
                        Message = "Sorry, you can type only number greater than 0, or you can /cancel command.",
                        StatusCode = StatusCodeEnum.Bad
                    }
                };
            }

            var category = await _categoryDocumentService.GetByIdAsync(user.Context.CategoryId);

            category.SupposedToSpentThisMonthInCents = number;
            category.SpentThisMonthInCents = 0;
            category.SpentInCents = 0;

            await _categoryDocumentService.UpdateAsync(category);

            user.Context.LastQuestion = QuestionsEnum.None;
            user.Context.CategoryId = null;

            await _userDocumentService.UpdateAsync(user);

            return new List<HandlerServiceResult>
            {
                new HandlerServiceResult
                {
                    Message = "Your category configured. Now you can create operations in this category by using /category command.",
                    StatusCode = StatusCodeEnum.Ok
                }
            };
        }

        public async Task<List<HandlerServiceResult>> HandleCategoryQuestion(
            string answer,
            User user)
        {
            List<HandlerServiceResult> result;

            try
            {
                result = await _questionsHandlerDictionary[user.Context.LastQuestion].Invoke(answer, user);
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

            if (categories != null && categories.Count > 0)
            {
                user.Context = new Context
                {
                    LastQuestion = QuestionsEnum.CategoryAction
                };

                await _userDocumentService.UpdateAsync(user);

                var categoriesString = string.Join("\n", categories.Select(c => "-" + c.Name));

                return new List<HandlerServiceResult>
                {
                    new HandlerServiceResult
                    {
                        Helper = new List<string>
                        {
                            "Add new category",
                            "Edit category",
                            "Delete category"
                        },
                        Message = string.Format("Here's your categories list:\n{0}\nYou can add new, edit or delete category.", categoriesString),
                        StatusCode = StatusCodeEnum.NeedKeyboard
                    }
                };
            }

            user.Context = new Context
            {
                LastQuestion = QuestionsEnum.AddNewCategoryOrNot
            };

            await _userDocumentService.UpdateAsync(user);

            return new List<HandlerServiceResult>
            {
                new HandlerServiceResult
                {
                    StatusCode = StatusCodeEnum.NeedKeyboard,
                    Helper = new List<string>
                    {
                        "Yes",
                        "No"
                    },
                    Message = "You don't have any categories yet. Do you want to add categories?"
                }
            };
        }
    }
}