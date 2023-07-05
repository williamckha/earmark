using Earmark.Backend.Models;
using System;

namespace Earmark.Backend.Services
{
    public interface IBudgetService
    {
        BudgetMonth GetBudgetMonth(int month, int year);

        int GetTotalBudgetedForMonthInCategory(Guid budgetMonthId, Guid categoryId);

        int GetTotalBalanceForMonthInCategory(Guid budgetMonthId, Guid categoryId);

        int GetTotalOverspentForMonth(Guid budgetMonthId);

        int GetTotalActivityForMonthInCategory(int month, int year, Guid categoryId);

        int GetTotalIncomeForMonth(int month, int year);

        BudgetMonth AddBudgetMonth(int month, int year);

        BalanceAmount AddBalanceAmount(Guid budgetMonthId, Guid categoryId, int amount);

        RolloverAmount AddRolloverAmount(Guid budgetMonthId, Guid categoryId, int amount);

        BudgetedAmount SetBudgetedAmount(Guid budgetMonthId, Guid categoryId, int amount);

        void UpdateBalanceAmounts(int month, int year, Guid categoryId, int activity);

        void UpdateTotalUnbudgetedAmounts(int month, int year);

        void UpdateTotalUnbudgetedAmounts();
    }
}
