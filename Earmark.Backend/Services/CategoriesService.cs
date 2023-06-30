using Earmark.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Earmark.Backend.Services
{
    public class CategoriesService : ICategoriesService
    {
        private IBudgetService _budgetService;

        public CategoriesService(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        public IEnumerable<CategoryGroup> GetCategoryGroups()
        {
            return _budgetService
                .GetBudget()
                .CategoryGroups;
        }

        public IEnumerable<Category> GetCategories()
        {
            return _budgetService
                .GetBudget()
                .CategoryGroups
                .SelectMany(x => x.Categories);
        }

        public CategoryGroup AddCategoryGroup(string name)
        {
            var budget = _budgetService.GetBudget();

            if (budget.CategoryGroups.Any(x => x.Name == name))
                throw new ArgumentException("Category group with the same name already exists.");

            var categoryGroup = new CategoryGroup()
            {
                Id = Guid.NewGuid(),
                Position = budget.CategoryGroups.Count(),
                Name = name,
                IsIncome = false,
                Budget = budget,
                Categories = new List<Category>()
            };

            budget.CategoryGroups.Add(categoryGroup);

            return categoryGroup;
        }

        public Category AddCategory(CategoryGroup categoryGroup, string name)
        {
            if (categoryGroup is null) throw new ArgumentNullException(nameof(categoryGroup));

            if (categoryGroup.Categories.Any(x => x.Name == name))
                throw new ArgumentException("Category with the same name already exists.");

            var category = new Category()
            {
                Id = Guid.NewGuid(),
                Position = categoryGroup.Categories.Count(),
                Name = name,
                IsIncome = false,
                Group = categoryGroup,
                BudgetedAmounts = new List<BudgetedAmount>(),
                BalanceAmounts = new List<BalanceAmount>(),
                RolloverAmounts = new List<RolloverAmount>(),
                Transactions = new List<Transaction>()
            };

            categoryGroup.Categories.Add(category);

            return category;
        }

        public void RemoveCategoryGroup(CategoryGroup categoryGroup)
        {
            if (categoryGroup is null) throw new ArgumentNullException(nameof(categoryGroup));

            while (categoryGroup.Categories.Any())
            {
                RemoveCategory(categoryGroup.Categories.First(), updateUnbudgetedAmounts: false);
            }

            var budget = _budgetService.GetBudget();
            budget.CategoryGroups.Remove(categoryGroup);
            categoryGroup.Budget = null;

            for (int position = 0; position < budget.CategoryGroups.Count(); position++)
            {
                budget.CategoryGroups[position].Position = position;
            }

            _budgetService.UpdateTotalUnbudgetedAmounts();
        }

        public void RemoveCategory(Category category)
        {
            RemoveCategory(category, updateUnbudgetedAmounts: true);
        }

        private void RemoveCategory(Category category, bool updateUnbudgetedAmounts)
        {
            if (category is null) throw new ArgumentNullException(nameof(category));

            while (category.BudgetedAmounts.Any())
            {
                var budgetedAmount = category.BudgetedAmounts.First();

                budgetedAmount.Month.BudgetedAmounts.Remove(budgetedAmount);
                budgetedAmount.Month = null;

                budgetedAmount.Category.BudgetedAmounts.Remove(budgetedAmount);
                budgetedAmount.Category = null;
            }

            while (category.BalanceAmounts.Any())
            {
                var balanceAmount = category.BalanceAmounts.First();

                balanceAmount.Month.BalanceAmounts.Remove(balanceAmount);
                balanceAmount.Month = null;

                balanceAmount.Category.BalanceAmounts.Remove(balanceAmount);
                balanceAmount.Category = null;
            }
            
            while (category.RolloverAmounts.Any())
            {
                var rolloverAmount = category.RolloverAmounts.First();

                rolloverAmount.Month.RolloverAmounts.Remove(rolloverAmount);
                rolloverAmount.Month = null;

                rolloverAmount.Category.RolloverAmounts.Remove(rolloverAmount);
                rolloverAmount.Category = null;
            }

            while (category.Transactions.Any())
            {
                var transaction = category.Transactions.First();
                category.Transactions.Remove(transaction);
                transaction.Category = null;
            }

            var categoryGroup = category.Group;
            categoryGroup.Categories.Remove(category);
            category.Group = null;

            for (int position = 0; position < categoryGroup.Categories.Count(); position++)
            {
                categoryGroup.Categories[position].Position = position;
            }

            if (updateUnbudgetedAmounts)
            {
                _budgetService.UpdateTotalUnbudgetedAmounts();
            }
        }
    }
}
