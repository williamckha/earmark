using Earmark.Backend.Models;

namespace Earmark.Backend.Services
{
    public interface IBudgetService
    {
        Budget GetBudget();

        BudgetMonth AddBudgetMonth(int month, int year);

        BudgetMonth GetBudgetMonth(int month, int year);

        decimal GetTotalBudgetedForMonth(BudgetMonth budgetMonth);

        decimal GetTotalBudgetedForMonth(BudgetMonth budgetMonth, CategoryGroup categoryGroup);

        decimal GetTotalBudgetedForMonth(BudgetMonth budgetMonth, Category category);

        decimal GetTotalBalanceForMonth(BudgetMonth budgetMonth);

        decimal GetTotalBalanceForMonth(BudgetMonth budgetMonth, CategoryGroup categoryGroup);

        decimal GetTotalBalanceForMonth(BudgetMonth budgetMonth, Category category);

        decimal GetTotalOverspentForMonth(BudgetMonth budgetMonth);

        BudgetedAmount SetBudgetedAmount(BudgetMonth budgetMonth, Category category, decimal amount);

        void UpdateBalanceAmounts(int month, int year, Category category, decimal activity);

        void UpdateTotalUnbudgetedAmounts(int month = 1, int year = 0);
    }
}
