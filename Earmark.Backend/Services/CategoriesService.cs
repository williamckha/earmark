using Earmark.Backend.Database;
using Earmark.Backend.Models;
using EntityFramework.DbContextScope.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Earmark.Backend.Services
{
    public class CategoriesService : ICategoriesService
    {
        private IDbContextScopeFactory _dbContextScopeFactory;
        private IBudgetService _budgetService;

        public CategoriesService(IDbContextScopeFactory dbContextScopeFactory, IBudgetService budgetService)
        {
            _dbContextScopeFactory = dbContextScopeFactory;
            _budgetService = budgetService;
        }

        public IEnumerable<CategoryGroup> GetCategoryGroups()
        {
            using (var dbContextScope = _dbContextScopeFactory.CreateReadOnly())
            {
                return dbContextScope.DbContexts.Get<AppDbContext>().CategoryGroups
                    .Include(x => x.Categories.OrderBy(x => x.Position))
                    .OrderBy(x => x.Position)
                    .AsNoTracking()
                    .ToList();
            }
        }

        public IEnumerable<Category> GetCategories()
        {
            using (var dbContextScope = _dbContextScopeFactory.CreateReadOnly())
            {
                return dbContextScope.DbContexts.Get<AppDbContext>().Categories
                    .OrderBy(x => x.Position)
                    .AsNoTracking()
                    .ToList();
            }
        }

        public CategoryGroup AddCategoryGroup(string name)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                if (dbContext.CategoryGroups.Any(x => x.Name == name))
                    throw new ArgumentException("Category group with the same name already exists.");

                var categoryGroup = new CategoryGroup()
                {
                    Position = dbContext.Categories.Count(),
                    Name = name,
                    IsIncome = false,
                    Categories = new List<Category>()
                };

                dbContext.CategoryGroups.Add(categoryGroup);
                dbContextScope.SaveChanges();
                return categoryGroup;
            }
        }

        public Category AddCategory(int categoryGroupId, string name)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();
                
                if (dbContext.Categories.Any(x => x.Name == name))
                    throw new ArgumentException("Category with the same name already exists.");

                var categoryGroup = dbContext.CategoryGroups
                    .Include(x => x.Categories)
                    .FirstOrDefault(x => x.Id == categoryGroupId);

                if (categoryGroup is null)
                    throw new ArgumentException("No category group with the specified ID was found.");

                var category = new Category()
                {
                    Position = categoryGroup.Categories.Count(),
                    Name = name,
                    IsIncome = false,
                    Group = categoryGroup
                };

                dbContext.Categories.Add(category);

                // Get the temporary ID of the added category.
                var categoryId = dbContext.Entry(category).Property(x => x.Id).CurrentValue;

                var budgetMonthIds = dbContext.BudgetMonths.Select(x => x.Id).ToList();
                foreach (var budgetMonthId in budgetMonthIds)
                {
                    _budgetService.AddBalanceAmount(budgetMonthId, categoryId, 0);
                }

                dbContextScope.SaveChanges();
                return category;
            }
        }

        public void RemoveCategoryGroup(int categoryGroupId)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                var categoryGroup = dbContext.CategoryGroups.Find(categoryGroupId);

                dbContext.CategoryGroups
                    .Where(x => x.Position > categoryGroup.Position)
                    .ExecuteUpdate(setters => setters.SetProperty(x => x.Position, x => x.Position - 1));

                dbContext.CategoryGroups.Remove(categoryGroup);

                dbContextScope.SaveChanges();
            }
        }

        public void RemoveCategory(int categoryId)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                var category = dbContext.Categories.Find(categoryId);
                dbContext.Entry(category).Reference(x => x.Group).Load();

                dbContext.Categories
                    .Where(x => x.Group.Id == category.Group.Id && x.Position > category.Position)
                    .ExecuteUpdate(setters => setters.SetProperty(x => x.Position, x => x.Position - 1));

                category.Group = null;

                dbContextScope.SaveChanges();
            }
        }
    }
}
