using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Earmark.Backend.Models;
using Earmark.Backend.Services;
using Earmark.Data.Messages;
using System;

namespace Earmark.ViewModels.Budget
{
    public partial class BudgetMonthCategoryViewModel : ObservableRecipient
    {
        private IBudgetService _budgetService;

        /// <summary>
        /// The total amount of money budgeted for the month in the category.
        /// </summary>
        [ObservableProperty]
        private int _totalBudgeted;

        /// <summary>
        /// The total sum of inflows and outflows for the month in the category.
        /// </summary>
        [ObservableProperty]
        private int _totalActivity;

        /// <summary>
        /// The total amount of money available for the month in the category.
        /// </summary>
        [ObservableProperty]
        private int _totalBalance;

        /// <summary>
        /// The unique ID that identifies the budget month.
        /// </summary>
        public int BudgetMonthId { get; }

        /// <summary>
        /// The unique ID that identifies the category.
        /// </summary>
        public int CategoryId { get; }

        /// <summary>
        /// The unique ID that identifies the category group of the category.
        /// </summary>
        public int CategoryGroupId { get; }

        /// <summary>
        /// The month of the budget month.
        /// </summary>
        public int Month { get; }

        /// <summary>
        /// The year of the budget month.
        /// </summary>
        public int Year { get; }

        public BudgetMonthCategoryViewModel(
            IBudgetService budgetService, 
            BudgetMonth budgetMonth, 
            Category category) : base(StrongReferenceMessenger.Default)
        {
            _budgetService = budgetService;

            BudgetMonthId = budgetMonth.Id;
            CategoryId = category.Id;
            CategoryGroupId = category.Group.Id;
            Month = budgetMonth.Month;
            Year = budgetMonth.Year;

            _totalBudgeted = _budgetService.GetTotalBudgetedForMonthInCategory(BudgetMonthId, CategoryId);
            _totalActivity = _budgetService.GetTotalActivityForMonthInCategory(Month, Year, CategoryId);
            _totalBalance = _budgetService.GetTotalBalanceForMonthInCategory(BudgetMonthId, CategoryId);

            IsActive = true;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            Messenger.Register<BudgetMonthCategoryViewModel, BudgetedAmountChangedMessage, int>(this, CategoryId, (r, m) =>
            {
                r.TotalBalance = r._budgetService.GetTotalBalanceForMonthInCategory(r.BudgetMonthId, r.CategoryId);
            });
        }

        partial void OnTotalBudgetedChanged(int value)
        {
            _budgetService.SetBudgetedAmount(BudgetMonthId, CategoryId, value);
            _budgetService.UpdateTotalUnbudgetedAmounts(Month, Year);

            Messenger.Send(new BudgetedAmountChangedMessage(), CategoryId);
            Messenger.Send(new BudgetedAmountChangedMessage(), CategoryGroupId);
            Messenger.Send(new BudgetedAmountChangedMessage(), nameof(BudgetMonthViewModel));
        }
    }
}
