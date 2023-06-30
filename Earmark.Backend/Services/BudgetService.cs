using Earmark.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Earmark.Backend.Services
{
    public class BudgetService : IBudgetService
    {
        private IAccountStore _accountStore;
        private IAccountDetailService _accountDetailService;

        private Budget _budget;

        public BudgetService(
            IAccountStore accountStore,
            IAccountDetailService accountDetailService)
        {
            _accountStore = accountStore;
            _accountDetailService = accountDetailService;
        }

        public Budget GetBudget()
        {
            if (_budget is null)
            {
                _budget = new Budget()
                {
                    Id = Guid.NewGuid(),
                    Months = new List<BudgetMonth>(),
                    CategoryGroups = new List<CategoryGroup>()
                };
            }

            return _budget;
        }

        public BudgetMonth GetBudgetMonth(int month, int year)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));

            return _budget.Months.FirstOrDefault(x => x.Month == month && x.Year == year);
        }

        public decimal GetTotalBudgetedForMonth(BudgetMonth budgetMonth)
        {
            if (budgetMonth is null) throw new ArgumentNullException(nameof(budgetMonth));

            return budgetMonth.BudgetedAmounts.Sum(x => x.Amount);
        }

        public decimal GetTotalBudgetedForMonth(BudgetMonth budgetMonth, CategoryGroup categoryGroup)
        {
            if (budgetMonth is null) throw new ArgumentNullException(nameof(budgetMonth));
            if (categoryGroup is null) throw new ArgumentNullException(nameof(categoryGroup));

            return budgetMonth.BudgetedAmounts
                .Where(x => x.Category.Group == categoryGroup)
                .Sum(x => x.Amount);
        }

        public decimal GetTotalBudgetedForMonth(BudgetMonth budgetMonth, Category category)
        {
            if (budgetMonth is null) throw new ArgumentNullException(nameof(budgetMonth));
            if (category is null) throw new ArgumentNullException(nameof(category));

            return budgetMonth.BudgetedAmounts
                .Where(x => x.Category == category)
                .Sum(x => x.Amount);
        }

        public decimal GetTotalBalanceForMonth(BudgetMonth budgetMonth)
        {
            if (budgetMonth is null) throw new ArgumentNullException(nameof(budgetMonth));

            return budgetMonth.BalanceAmounts.Sum(x => x.Amount);
        }

        public decimal GetTotalBalanceForMonth(BudgetMonth budgetMonth, CategoryGroup categoryGroup)
        {
            if (budgetMonth is null) throw new ArgumentNullException(nameof(budgetMonth));
            if (categoryGroup is null) throw new ArgumentNullException(nameof(categoryGroup));

            return budgetMonth.BalanceAmounts
                .Where(x => x.Category.Group == categoryGroup)
                .Sum(x => x.Amount);
        }

        public decimal GetTotalBalanceForMonth(BudgetMonth budgetMonth, Category category)
        {
            if (budgetMonth is null) throw new ArgumentNullException(nameof(budgetMonth));
            if (category is null) throw new ArgumentNullException(nameof(category));

            return budgetMonth.BalanceAmounts
                .Where(x => x.Category == category)
                .Sum(x => x.Amount);
        }

        public decimal GetTotalOverspentForMonth(BudgetMonth budgetMonth)
        {
            if (budgetMonth is null) throw new ArgumentNullException(nameof(budgetMonth));

            // The month's balance amounts cannot be summed to determine overspending since
            // balance amounts rollover each month. (overspending from last month would also
            // count as overspending in the current month). Instead, we look at the month's
            // transactions to determine how much was spent in the specified month.

            var totalOverspent = _accountStore
                .GetAccounts()
                .SelectMany(x => x.Transactions)
                .Where(x =>
                    x.Category is not null &&
                    x.Date.Year == budgetMonth.Year && x.Date.Month == budgetMonth.Month)
                .GroupBy(x => x.Category)
                .Select(transactions =>
                {
                    var rolloverAmount = budgetMonth.RolloverAmounts
                        .FirstOrDefault(x => x.Category == transactions.Key)?.Amount ?? decimal.Zero;
                    var budgetedAmount = budgetMonth.BudgetedAmounts
                        .FirstOrDefault(x => x.Category == transactions.Key)?.Amount ?? decimal.Zero;
                    return Math.Min(Math.Max(rolloverAmount, 0) + budgetedAmount + transactions.Sum(x => x.Amount), 0);
                })
                .Sum();

            return Math.Abs(totalOverspent);
        }

        public BudgetMonth AddBudgetMonth(int month, int year)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));
            if (_budget.Months.Any(x => x.Month == month && x.Year == year))
                throw new ArgumentException("Month already exists in budget.");

            var budgetMonth = new BudgetMonth()
            {
                Id = Guid.NewGuid(),
                Month = month,
                Year = year,
                TotalUnbudgeted = decimal.Zero,
                Budget = _budget,
                BudgetedAmounts = new List<BudgetedAmount>(),
                BalanceAmounts = new List<BalanceAmount>(),
                RolloverAmounts = new List<RolloverAmount>()
            };

            // Get the budget month immediately behind the new budget month.
            var previousBudgetMonth = _budget.Months
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .LastOrDefault(x => (x.Year == budgetMonth.Year && x.Month < budgetMonth.Month) || (x.Year < budgetMonth.Year));

            if (previousBudgetMonth is not null)
            {
                // Rollover balance amounts from the previous month to the new month.
                foreach (var previousBalanceAmount in previousBudgetMonth.BalanceAmounts)
                {
                    AddBalanceAmount(budgetMonth, previousBalanceAmount.Category, previousBalanceAmount.Amount);
                    AddRolloverAmount(budgetMonth, previousBalanceAmount.Category, previousBalanceAmount.Amount);
                }

                // Rollover total unbudgeted from the previous month to the new month.
                budgetMonth.TotalUnbudgeted = previousBudgetMonth.TotalUnbudgeted;
            }

            _budget.Months.Add(budgetMonth);

            return budgetMonth;
        }

        public BudgetedAmount SetBudgetedAmount(BudgetMonth budgetMonth, Category category, decimal amount)
        {
            if (budgetMonth is null) throw new ArgumentNullException(nameof(budgetMonth));
            if (category is null) throw new ArgumentNullException(nameof(category));

            decimal budgetedAmountChange = amount;

            var budgetedAmount = budgetMonth.BudgetedAmounts.FirstOrDefault(x => x.Category == category);
            if (budgetedAmount is not null)
            {
                budgetedAmountChange = amount - budgetedAmount.Amount;
                budgetedAmount.Amount = amount;
            }
            else
            {
                budgetedAmount = new BudgetedAmount()
                {
                    Id = Guid.NewGuid(),
                    Amount = amount,
                    Month = budgetMonth,
                    Category = category
                };

                budgetMonth.BudgetedAmounts.Add(budgetedAmount);
                category.BudgetedAmounts.Add(budgetedAmount);
            }

            UpdateBalanceAmounts(budgetMonth.Month, budgetMonth.Year, category, budgetedAmountChange);
            UpdateTotalUnbudgetedAmounts(budgetMonth.Month, budgetMonth.Year);

            return budgetedAmount;
        }

        public void UpdateBalanceAmounts(int month, int year, Category category, decimal activity)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));
            if (category is null) throw new ArgumentNullException(nameof(category));

            var budgetMonth =
                _budget.Months.FirstOrDefault(x => x.Month == month && x.Year == year) ??
                AddBudgetMonth(month, year);

            // Get all months after the specified month.
            var subsequentBudgetMonths = _budget.Months
                .Where(x => (x.Month > month && x.Year == year) || (x.Year > year));

            var balanceAmount =
                budgetMonth.BalanceAmounts.FirstOrDefault(x => x.Category == category) ??
                AddBalanceAmount(budgetMonth, category, decimal.Zero);

            // This makes sure we don't rollover negative balances.
            var rolledOverActivity = Math.Max(balanceAmount.Amount + activity, 0) - Math.Max(balanceAmount.Amount, 0);

            balanceAmount.Amount += activity;

            foreach (var subsequentBudgetMonth in subsequentBudgetMonths)
            {
                var subsequentBalanceAmount =
                    subsequentBudgetMonth.BalanceAmounts.FirstOrDefault(x => x.Category == category) ??
                    AddBalanceAmount(subsequentBudgetMonth, category, decimal.Zero);

                subsequentBalanceAmount.Amount += rolledOverActivity;

                var subsequentRolloverAmount =
                    subsequentBudgetMonth.RolloverAmounts.FirstOrDefault(x => x.Category == category) ??
                    AddRolloverAmount(subsequentBudgetMonth, category, decimal.Zero);

                subsequentRolloverAmount.Amount += rolledOverActivity;
            }
        }

        public void UpdateTotalUnbudgetedAmounts(int month, int year)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));

            var budgetMonths = _budget.Months
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            var lastBudgetMonth = budgetMonths
                .LastOrDefault(x => (x.Month < month && x.Year == year) || (x.Year < year));

            // Get all months at or after the specified month.
            var budgetMonthsToUpdate = budgetMonths
                .Where(x => (x.Month >= month && x.Year == year) || (x.Year > year));

            foreach (var budgetMonth in budgetMonthsToUpdate)
            {
                var unbudgetedLastMonth = decimal.Zero;
                var overspentLastMonth = decimal.Zero;
                if (lastBudgetMonth is not null)
                {
                    unbudgetedLastMonth = lastBudgetMonth.TotalUnbudgeted;
                    overspentLastMonth = GetTotalOverspentForMonth(lastBudgetMonth);
                }

                var totalIncome = _accountDetailService.GetTotalIncomeForMonth(budgetMonth.Month, budgetMonth.Year);
                var totalBudgeted = GetTotalBudgetedForMonth(budgetMonth);

                var totalUnbudgeted = unbudgetedLastMonth - overspentLastMonth + totalIncome - totalBudgeted;
                budgetMonth.TotalUnbudgeted = totalUnbudgeted;

                lastBudgetMonth = budgetMonth;
            }
        }

        private BalanceAmount AddBalanceAmount(BudgetMonth budgetMonth, Category category, decimal amount)
        {
            var balanceAmount = new BalanceAmount()
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                Month = budgetMonth,
                Category = category
            };

            budgetMonth.BalanceAmounts.Add(balanceAmount);
            category.BalanceAmounts.Add(balanceAmount);

            return balanceAmount;
        }
        
        private RolloverAmount AddRolloverAmount(BudgetMonth budgetMonth, Category category, decimal amount)
        {
            var rolloverAmount = new RolloverAmount()
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                Month = budgetMonth,
                Category = category
            };

            budgetMonth.RolloverAmounts.Add(rolloverAmount);
            category.RolloverAmounts.Add(rolloverAmount);

            return rolloverAmount;
        }
    }
}
