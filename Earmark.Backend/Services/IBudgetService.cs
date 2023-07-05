using Earmark.Backend.Models;
using System;

namespace Earmark.Backend.Services
{
    public interface IBudgetService
    {
        /// <summary>
        /// Gets the budget month with the specified month and year.
        /// </summary>
        /// <param name="month">The month of the budget month.</param>
        /// <param name="year">The year of the budget month.</param>
        /// <returns>The budget month, or null if no such budget month exists in the database.</returns>
        BudgetMonth GetBudgetMonth(int month, int year);

        /// <summary>
        /// Gets the total amount budgeted for the specified month in the given category.
        /// </summary>
        /// <param name="budgetMonthId">The ID of the budget month.</param>
        /// <param name="categoryId">The ID of the category.</param>
        /// <returns>The total amount budgeted for the month in the category.</returns>
        int GetTotalBudgetedForMonthInCategory(Guid budgetMonthId, Guid categoryId);

        /// <summary>
        /// Gets the total balance for the specified month in the given category.
        /// </summary>
        /// <param name="budgetMonthId">The ID of the budget month.</param>
        /// <param name="categoryId">The ID of the category.</param>
        /// <returns>The total balance for the month in the category.</returns>
        int GetTotalBalanceForMonthInCategory(Guid budgetMonthId, Guid categoryId);

        /// <summary>
        /// Gets the total amount overspent for the specified month.
        /// </summary>
        /// <param name="budgetMonthId">The ID of the budget month.</param>
        /// <returns>The total amount overspent for the month.</returns>
        int GetTotalOverspentForMonth(Guid budgetMonthId);

        /// <summary>
        /// Gets the total activity for the specified month in the given category.
        /// The total activity is the sum of all transactions that have been made in the 
        /// category for the month.
        /// </summary>
        /// <param name="month">The month of the budget month.</param>
        /// <param name="year">The year of the budget month.</param>
        /// <param name="categoryId">The ID of the category.</param>
        /// <returns>The total activity for the month in the category.</returns>
        int GetTotalActivityForMonthInCategory(int month, int year, Guid categoryId);

        /// <summary>
        /// Gets the total income for the specified month.
        /// </summary>
        /// <param name="month">The month of the budget month.</param>
        /// <param name="year">The year of the budget month.</param>
        /// <returns>The total income for the month.</returns>
        int GetTotalIncomeForMonth(int month, int year);

        /// <summary>
        /// Adds a budget month.
        /// </summary>
        /// <param name="month">The month of the budget month.</param>
        /// <param name="year">The year of the budget month.</param>
        /// <returns>The added budget month.</returns>
        BudgetMonth AddBudgetMonth(int month, int year);

        /// <summary>
        /// Adds a balance amount for the specified month and category.
        /// </summary>
        /// <param name="budgetMonthId">The ID of the budget month.</param>
        /// <param name="categoryId">The ID of the category.</param>
        /// <param name="amount">The balance for the month in the category.</param>
        /// <returns>The added balance amount.</returns>
        BalanceAmount AddBalanceAmount(Guid budgetMonthId, Guid categoryId, int amount);

        /// <summary>
        /// Adds a budgeted amount for the specified month and category.
        /// </summary>
        /// <param name="budgetMonthId">The ID of the budget month.</param>
        /// <param name="categoryId">The ID of the category.</param>
        /// <param name="amount">The amount budgeted for the month in the category.</param>
        /// <returns>The added budgeted amount.</returns>
        BudgetedAmount SetBudgetedAmount(Guid budgetMonthId, Guid categoryId, int amount);

        /// <summary>
        /// Updates balance amounts in the specified category for all budget months 
        /// on and after the specified month.
        /// </summary>
        /// <param name="month">The month of the budget month.</param>
        /// <param name="year">The year of the budget month.</param>
        /// <param name="categoryId">The ID of the category.</param>
        /// <param name="activity">The amount to increment or decrement the balance amounts by.</param>
        void UpdateBalanceAmounts(int month, int year, Guid categoryId, int activity);

        /// <summary>
        /// Calculates and updates the total amount unbudgeted for all budget months
        /// on and after the specified month.
        /// </summary>
        /// <param name="month">The month of the budget month.</param>
        /// <param name="year">The year of the budget month.</param>
        void UpdateTotalUnbudgetedAmounts(int month, int year);

        /// <summary>
        /// Calculates and updates the total amount unbudgeted for all budget months.
        /// </summary>
        void UpdateTotalUnbudgetedAmounts();
    }
}
