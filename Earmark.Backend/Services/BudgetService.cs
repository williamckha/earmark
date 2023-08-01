using Earmark.Backend.Database;
using Earmark.Backend.Models;
using EntityFramework.DbContextScope.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Earmark.Backend.Services
{
    public class BudgetService : IBudgetService
    {
        private IDbContextScopeFactory _dbContextScopeFactory;

        public BudgetService(IDbContextScopeFactory dbContextScopeFactory)
        {
            _dbContextScopeFactory = dbContextScopeFactory;
        }

        public IEnumerable<BudgetMonth> GetBudgetMonths()
        {
            using (var dbContextScope = _dbContextScopeFactory.CreateReadOnly())
            {
                return dbContextScope.DbContexts.Get<AppDbContext>().BudgetMonths
                    .AsNoTracking()
                    .ToList();
            }
        }

        public BudgetMonth GetBudgetMonth(int month, int year)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));

            using (var dbContextScope = _dbContextScopeFactory.CreateReadOnly())
            {
                return dbContextScope.DbContexts.Get<AppDbContext>().BudgetMonths
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Month == month && x.Year == year);
            }
        }

        public int GetTotalBudgetedForMonthInCategory(int budgetMonthId, int categoryId)
        {
            using (var dbContextScope = _dbContextScopeFactory.CreateReadOnly())
            {
                return dbContextScope.DbContexts.Get<AppDbContext>().BudgetedAmounts
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Month.Id == budgetMonthId && x.Category.Id == categoryId)?.Amount ?? 0;
            }
        }

        public int GetTotalBalanceForMonthInCategory(int budgetMonthId, int categoryId)
        {
            using (var dbContextScope = _dbContextScopeFactory.CreateReadOnly())
            {
                return dbContextScope.DbContexts.Get<AppDbContext>().BalanceAmounts
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Month.Id == budgetMonthId && x.Category.Id == categoryId)?.Amount ?? 0;
            }
        }

        public int GetTotalOverspentForMonth(int budgetMonthId)
        {
            using (var dbContextScope = _dbContextScopeFactory.CreateReadOnly())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                var budgetMonth = dbContext.BudgetMonths.Find(budgetMonthId);
                dbContext.Entry(budgetMonth).Collection(x => x.BalanceAmounts).Load();

                var totalOverspent = budgetMonth.BalanceAmounts.Select(x => Math.Min(x.Amount, 0)).Sum();
                return Math.Abs(totalOverspent);
            }
        }

        public int GetTotalActivityForMonthInCategory(int month, int year, int categoryId)
        {
            using (var dbContextScope = _dbContextScopeFactory.CreateReadOnly())
            {
                return dbContextScope.DbContexts.Get<AppDbContext>().Transactions
                    .Where(x =>
                        x.Date.Year == year &&
                        x.Date.Month == month &&
                        x.Category != null &&
                        x.Category.Id == categoryId)
                    .AsNoTracking()
                    .Sum(x => x.Amount);
            }
        }

        public int GetTotalIncomeForMonth(int month, int year)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));

            using (var dbContextScope = _dbContextScopeFactory.CreateReadOnly())
            {
                return dbContextScope.DbContexts.Get<AppDbContext>().Transactions
                    .Where(x =>
                        x.Date.Year == year &&
                        x.Date.Month == month &&
                        x.Category != null &&
                        x.Category.IsIncome == true)
                    .Sum(x => x.Amount);
            }
        }

        public BudgetMonth AddBudgetMonth(int month, int year)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                if (dbContext.BudgetMonths.Any(x => x.Month == month && x.Year == year))
                    throw new ArgumentException("Month already exists in budget.");

                var budgetMonth = new BudgetMonth()
                {
                    Month = month,
                    Year = year,
                    TotalUnbudgeted = 0
                };

                dbContext.BudgetMonths.Add(budgetMonth);
                
                // Get the temporary ID of the added budget month.
                var budgetMonthId = dbContext.Entry(budgetMonth).Property(x => x.Id).CurrentValue;

                // Try getting the budget month immediately behind the new budget month.
                var previousBudgetMonth = dbContext.BudgetMonths
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .LastOrDefault(x => (x.Year == budgetMonth.Year && x.Month < budgetMonth.Month) || (x.Year < budgetMonth.Year));

                if (previousBudgetMonth is not null)
                {
                    dbContext.Entry(previousBudgetMonth)
                        .Collection(x => x.BalanceAmounts)
                        .Query()
                        .Include(x => x.Category)
                        .Load();

                    // Rollover balance amounts from the previous month to the new month.
                    foreach (var previousBalanceAmount in previousBudgetMonth.BalanceAmounts)
                    {
                        AddBalanceAmount(budgetMonthId, previousBalanceAmount.Category.Id, Math.Max(previousBalanceAmount.Amount, 0));
                    }

                    // Rollover total unbudgeted from the previous month to the new month.
                    budgetMonth.TotalUnbudgeted = previousBudgetMonth.TotalUnbudgeted;
                }
                else
                {
                    // Add empty balance amounts in all categories to the new month.
                    var categoryIds = dbContext.Categories.Select(x => x.Id).ToList();
                    foreach (var categoryId in categoryIds)
                    {
                        AddBalanceAmount(budgetMonthId, categoryId, 0);
                    }
                }

                dbContextScope.SaveChanges();
                return budgetMonth;
            }
        }

        public BalanceAmount AddBalanceAmount(int budgetMonthId, int categoryId, int amount)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                var budgetMonth = dbContext.BudgetMonths.Find(budgetMonthId);
                var category = dbContext.Categories.Find(categoryId);

                var balanceAmount = new BalanceAmount()
                {
                    Amount = amount,
                    RolloverAmount = amount,
                    Month = budgetMonth,
                    Category = category
                };

                dbContext.BalanceAmounts.Add(balanceAmount);
                dbContextScope.SaveChanges();
                return balanceAmount;
            }
        }

        public BudgetedAmount SetBudgetedAmount(int budgetMonthId, int categoryId, int amount)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                int budgetedAmountChange = amount;

                var budgetMonth = dbContext.BudgetMonths.Find(budgetMonthId);
                var budgetedAmount = dbContext.BudgetedAmounts
                    .FirstOrDefault(x => x.Month.Id == budgetMonthId && x.Category.Id == categoryId);

                if (budgetedAmount is not null)
                {
                    budgetedAmountChange = amount - budgetedAmount.Amount;
                    budgetedAmount.Amount = amount;
                }
                else
                {
                    budgetedAmount = new BudgetedAmount()
                    {
                        Amount = amount,
                        Month = budgetMonth,
                        Category = dbContext.Categories.Find(categoryId)
                    };

                    dbContext.BudgetedAmounts.Add(budgetedAmount);
                }

                UpdateBalanceAmounts(budgetMonth.Month, budgetMonth.Year, categoryId, budgetedAmountChange);

                dbContextScope.SaveChanges();
                return budgetedAmount;
            }
        }
        
        public void UpdateBalanceAmounts(int month, int year, int categoryId, int activity)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                // Ensure that a budget month exists so that we know there is a balance amount
                // associated with the month.
                if (!dbContext.BudgetMonths.Any(x => x.Month == month && x.Year == year))
                {
                    AddBudgetMonth(month, year);
                }

                // Get all balance amounts on or after the specified month in the category.
                var balanceAmounts = dbContext.BalanceAmounts
                    .Where(x => x.Category.Id == categoryId)
                    .Where(x => (x.Month.Year == year && x.Month.Month >= month) || (x.Month.Year > year))
                    .OrderBy(x => x.Month.Year)
                    .ThenBy(x => x.Month.Month)
                    .ToList();

                balanceAmounts.First().Amount += activity;

                // Keep track of the balance amount before the current balance amount while looping.
                var lastBalanceAmount = balanceAmounts.First();

                foreach (var balanceAmount in balanceAmounts.Skip(1))
                {
                    // Find how much was added/subtracted from the rollover for this balance amount.
                    var movement = balanceAmount.Amount - balanceAmount.RolloverAmount;

                    // Only positive balances are rolled over.
                    var rollover = Math.Max(lastBalanceAmount.Amount, 0);

                    balanceAmount.RolloverAmount = rollover;
                    balanceAmount.Amount = movement + rollover;

                    lastBalanceAmount = balanceAmount;
                }

                dbContextScope.SaveChanges();
            }
        }

        public void UpdateTotalUnbudgetedAmounts(int month, int year)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                var budgetMonths = dbContext.BudgetMonths
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .ToList();

                // Get all months on or after the specified month.
                var budgetMonthsToUpdate = budgetMonths
                    .Where(x => (x.Month >= month && x.Year == year) || (x.Year > year));

                // Keep track of the budget month before the current budget month while looping.
                var lastBudgetMonth = budgetMonths
                    .LastOrDefault(x => (x.Month < month && x.Year == year) || (x.Year < year));

                foreach (var budgetMonth in budgetMonthsToUpdate)
                {
                    int unbudgetedLastMonth = 0;
                    int overspentLastMonth = 0;
                    if (lastBudgetMonth is not null)
                    {
                        unbudgetedLastMonth = lastBudgetMonth.TotalUnbudgeted;
                        overspentLastMonth = GetTotalOverspentForMonth(lastBudgetMonth.Id);
                    }

                    var totalIncome = GetTotalIncomeForMonth(budgetMonth.Month, budgetMonth.Year);
                    var totalBudgeted = dbContext.BudgetedAmounts.Where(x => x.Month.Id == budgetMonth.Id).Sum(x => x.Amount);

                    budgetMonth.TotalUnbudgeted = unbudgetedLastMonth - overspentLastMonth + totalIncome - totalBudgeted;

                    lastBudgetMonth = budgetMonth;
                }

                dbContextScope.SaveChanges();
            }
        }

        public void UpdateTotalUnbudgetedAmounts()
        {
            UpdateTotalUnbudgetedAmounts(1, 0);
        }
    }
}
