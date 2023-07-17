using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Earmark.Backend.Helpers;
using Earmark.Backend.Models;
using Earmark.Backend.Services;
using Earmark.Data.Messages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Earmark.ViewModels.Budget
{
    public partial class BudgetMonthViewModel : ObservableRecipient
    {
        private IBudgetService _budgetService;
        private ICategoriesService _categoriesService;

        private BudgetMonth _budgetMonth;

        /// <summary>
        /// The total amount of money budgeted for the month.
        /// </summary>
        [ObservableProperty]
        private int _totalBudgeted;

        /// <summary>
        /// The total sum of inflows and outflows for the month.
        /// </summary>
        [ObservableProperty]
        private int _totalActivity;

        /// <summary>
        /// The total amount of money available for the month.
        /// </summary>
        [ObservableProperty]
        private int _totalBalance;

        /// <summary>
        /// The total amount of money unbudgeted last month that was carried forward to
        /// the current month.
        /// </summary>
        [ObservableProperty]
        private int _unbudgetedLastMonth;

        /// <summary>
        /// The total amount of money overspent last month that was carried forward to
        /// the current month.
        /// </summary>
        [ObservableProperty]
        private int _overspentLastMonth;

        /// <summary>
        /// The total income for the month.
        /// </summary>
        [ObservableProperty]
        private int _totalIncome;

        /// <summary>
        /// The total amount of money unbudgeted for the month.
        /// </summary>
        [ObservableProperty]
        private int _totalUnbudgeted;

        /// <summary>
        /// The unique ID that identifies the budget month.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The month of the budget month.
        /// </summary>
        public int Month { get; }

        /// <summary>
        /// The year of the budget month.
        /// </summary>
        public int Year { get; }

        /// <summary>
        /// Whether the month is the current month of the current year.
        /// </summary>
        public bool IsCurrentMonth
        {
            get
            {
                var currentDate = DateTime.Now;
                return currentDate.Month == Month && currentDate.Year == Year;
            }
        }

        /// <summary>
        /// The category groups for the month.
        /// </summary>
        public ObservableCollection<BudgetMonthCategoryGroupViewModel> CategoryGroups { get; }

        /// <summary>
        /// The collection view source that adds grouping support to CategoryGroups.
        /// </summary>
        public CollectionViewSource CategoryGroupsCVS { get; }

        public BudgetMonthViewModel(
            IBudgetService budgetService, 
            ICategoriesService categoriesService,
            BudgetMonth budgetMonth) : base(StrongReferenceMessenger.Default)
        {
            _budgetService = budgetService;
            _categoriesService = categoriesService;
            _budgetMonth = budgetMonth;

            Id = budgetMonth.Id;
            Month = budgetMonth.Month;
            Year = budgetMonth.Year;

            CategoryGroups = new ObservableCollection<BudgetMonthCategoryGroupViewModel>();
            CategoryGroupsCVS = new CollectionViewSource()
            {
                Source = CategoryGroups,
                IsSourceGrouped = true,
                ItemsPath = new PropertyPath(nameof(BudgetMonthCategoryGroupViewModel.Categories))
            };

            foreach (var categoryGroup in _categoriesService.GetCategoryGroups())
            {
                CategoryGroups.Add(new BudgetMonthCategoryGroupViewModel(_budgetService, budgetMonth, categoryGroup));
            }

            UpdateCalculatedAndActivityProperties();

            IsActive = true;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            Messenger.Register<BudgetMonthViewModel, BudgetedAmountChangedMessage, string>(this, nameof(BudgetMonthViewModel), (r, m) =>
            {
                r.UpdateCalculatedProperties();
            });

            Messenger.Register<BudgetMonthViewModel, CategoryGroupAddedMessage>(this, (r, m) =>
            {
                r.CategoryGroups.Add(new BudgetMonthCategoryGroupViewModel(r._budgetService, r._budgetMonth, m.CategoryGroup));
            });

            Messenger.Register<BudgetMonthViewModel, CategoryGroupRemovedMessage>(this, (r, m) =>
            {
                var categoryGroupViewModel = r.CategoryGroups.First(x => x.CategoryGroupId == m.CategoryGroupId);

                categoryGroupViewModel.IsActive = false;
                r.CategoryGroups.Remove(categoryGroupViewModel);

                r.UpdateCalculatedAndActivityProperties();
            });

            Messenger.Register<BudgetMonthViewModel, CategoryRemovedMessage, string>(this, nameof(BudgetMonthViewModel), (r, m) =>
            {
                r.UpdateCalculatedAndActivityProperties();
            });
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            foreach (var categoryGroup in CategoryGroups)
            {
                categoryGroup.IsActive = false;
            }
        }

        [RelayCommand]
        public void CopyBudgetedAmountsFromLastMonth()
        {
            //var lastBudgetMonth = GetLastBudgetMonth();
            //if (lastBudgetMonth is null)
            //{
            //    ZeroAllBudgetedAmounts();
            //    return;
            //}

            //foreach (var category in _categoriesService.GetCategoryGroups().SelectMany(x => x.Categories))
            //{
            //    var lastMonthBudgetedAmount = _budgetService.GetTotalBudgetedForMonth(lastBudgetMonth, category);
            //    if (_budgetService.GetTotalBudgetedForMonth(_budgetMonth, category) != lastMonthBudgetedAmount)
            //    {
            //        _budgetService.SetBudgetedAmount(_budgetMonth.Id, category.Id, lastMonthBudgetedAmount);
            //    }
            //}

            //Messenger.Send(new BudgetedAmountResetMessage(_budgetMonth));
        }

        [RelayCommand]
        public void ZeroAllBudgetedAmounts()
        {
            //foreach (var category in _categoriesService.GetCategoryGroups().SelectMany(x => x.Categories))
            //{
            //    if (_budgetService.GetTotalBudgetedForMonthInCategory(_budgetMonth.Id, category.Id) != 0)
            //    {
            //        _budgetService.SetBudgetedAmount(_budgetMonth.Id, category.Id, 0);
            //    }
            //}

            //Messenger.Send(new BudgetedAmountResetMessage(_budgetMonth));
        }

        private void UpdateCalculatedProperties()
        {
            TotalBudgeted = CategoryGroups.Sum(x => x.TotalBudgeted);
            TotalBalance = CategoryGroups.Sum(x => x.TotalBalance);
            TotalUnbudgeted = _budgetService.GetBudgetMonth(Month, Year).TotalUnbudgeted;

            (int lastMonth, int yearOfLastMonth) = DateTimeHelper.GetPreviousMonth(Month, Year);
            var lastBudgetMonth = _budgetService.GetBudgetMonth(lastMonth, yearOfLastMonth);

            UnbudgetedLastMonth = lastBudgetMonth?.TotalUnbudgeted ?? 0;
            OverspentLastMonth = (lastBudgetMonth is not null) ?
                _budgetService.GetTotalOverspentForMonth(lastBudgetMonth.Id) : 0;
        }

        private void UpdateCalculatedAndActivityProperties()
        {
            UpdateCalculatedProperties();
            TotalActivity = CategoryGroups.Sum(x => x.TotalActivity);
            TotalIncome = _budgetService.GetTotalIncomeForMonth(Month, Year);
        }
    }
}
