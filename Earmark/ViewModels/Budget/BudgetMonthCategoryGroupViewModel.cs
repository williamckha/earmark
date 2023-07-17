using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Earmark.Backend.Models;
using Earmark.Backend.Services;
using Earmark.Data.Messages;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Earmark.ViewModels.Budget
{
    public partial class BudgetMonthCategoryGroupViewModel : ObservableRecipient
    {
        private IBudgetService _budgetService;

        private BudgetMonth _budgetMonth;

        /// <summary>
        /// The total amount of money budgeted for the category group.
        /// </summary>
        [ObservableProperty]
        private int _totalBudgeted;

        /// <summary>
        /// The total sum of inflows and outflows for the category group.
        /// </summary>
        [ObservableProperty]
        private int _totalActivity;

        /// <summary>
        /// The total amount of money available for the category group.
        /// </summary>
        [ObservableProperty]
        private int _totalBalance;

        /// <summary>
        /// The unique ID that identifies the category group.
        /// </summary>
        public int CategoryGroupId { get; }

        /// <summary>
        /// The categories contained in the category group.
        /// </summary>
        public ObservableCollection<BudgetMonthCategoryViewModel> Categories { get; }

        public BudgetMonthCategoryGroupViewModel(
            IBudgetService budgetService, 
            BudgetMonth budgetMonth, 
            CategoryGroup categoryGroup) : base(StrongReferenceMessenger.Default)
        {
            _budgetService = budgetService;
            _budgetMonth = budgetMonth;

            CategoryGroupId = categoryGroup.Id;

            Categories = new ObservableCollection<BudgetMonthCategoryViewModel>();

            foreach (var category in categoryGroup.Categories)
            {
                Categories.Add(new BudgetMonthCategoryViewModel(_budgetService, _budgetMonth, category));
            }

            UpdateCalculatedAndActivityProperties();

            IsActive = true;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            Messenger.Register<BudgetMonthCategoryGroupViewModel, BudgetedAmountChangedMessage, int>(this, CategoryGroupId, (r, m) =>
            {
                r.UpdateCalculatedProperties();
            });

            Messenger.Register<BudgetMonthCategoryGroupViewModel, CategoryAddedMessage, int>(this, CategoryGroupId, (r, m) =>
            {
                r.Categories.Add(new BudgetMonthCategoryViewModel(r._budgetService, r._budgetMonth, m.Category));
            });

            Messenger.Register<BudgetMonthCategoryGroupViewModel, CategoryRemovedMessage, int>(this, CategoryGroupId, (r, m) =>
            {
                var categoryViewModel = r.Categories.First(x => x.CategoryId == m.CategoryId);

                categoryViewModel.IsActive = false;
                r.Categories.Remove(categoryViewModel);

                r.UpdateCalculatedAndActivityProperties();
            });
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            foreach (var category in Categories)
            {
                category.IsActive = false;
            }
        }

        private void UpdateCalculatedProperties()
        {
            TotalBudgeted = Categories.Sum(x => x.TotalBudgeted);
            TotalBalance = Categories.Sum(x => x.TotalBalance);
        }

        private void UpdateCalculatedAndActivityProperties()
        {
            UpdateCalculatedProperties();
            TotalActivity = Categories.Sum(x => x.TotalActivity);
        }
    }
}
