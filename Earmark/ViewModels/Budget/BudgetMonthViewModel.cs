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
        private IAccountDetailService _accountDetailService;
        private IBudgetService _budgetService;

        private BudgetMonth _budgetMonth;

        /// <summary>
        /// The month.
        /// </summary>
        public int Month => _budgetMonth.Month;

        /// <summary>
        /// The year of the month.
        /// </summary>
        public int Year => _budgetMonth.Year;

        /// <summary>
        /// Whether the month is the current month of the current year.
        /// </summary>
        public bool IsCurrentMonth
        {
            get
            {
                var currentDate = DateTimeOffset.Now;
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

        /// <summary>
        /// The total amount of money unbudgeted last month that was carried forward to
        /// the current month.
        /// </summary>
        public decimal UnbudgetedLastMonth => GetLastBudgetMonth()?.TotalUnbudgeted ?? decimal.Zero;

        /// <summary>
        /// The total amount of money overspent last month that was carried forward to
        /// the current month.
        /// </summary>
        public decimal OverspentLastMonth
        {
            get
            {
                return (GetLastBudgetMonth() is BudgetMonth lastBudgetMonth) ? 
                    _budgetService.GetTotalOverspentForMonth(lastBudgetMonth) : decimal.Zero;
            }
        }

        /// <summary>
        /// The total amount of money budgeted for the month.
        /// </summary>
        public decimal TotalBudgeted => _budgetService.GetTotalBudgetedForMonth(_budgetMonth);

        /// <summary>
        /// The total sum of inflows and outflows for the month.
        /// </summary>
        public decimal TotalActivity => _accountDetailService.GetTotalActivityForMonth(_budgetMonth.Month, _budgetMonth.Year);

        /// <summary>
        /// The total amount of money available for the month.
        /// </summary>
        public decimal TotalBalance => _budgetService.GetTotalBalanceForMonth(_budgetMonth);

        /// <summary>
        /// The total income for the month.
        /// </summary>
        public decimal TotalIncome => _accountDetailService.GetTotalIncomeForMonth(_budgetMonth.Month, _budgetMonth.Year);

        /// <summary>
        /// The total amount of money unbudgeted for the month.
        /// </summary>
        public decimal TotalUnbudgeted => _budgetMonth.TotalUnbudgeted;

        public BudgetMonthViewModel(
            IAccountDetailService accountDetailService, 
            IBudgetService budgetService, 
            BudgetMonth budgetMonth) : base(StrongReferenceMessenger.Default)
        {
            _accountDetailService = accountDetailService;
            _budgetService = budgetService;

            _budgetMonth = budgetMonth;

            CategoryGroups = new ObservableCollection<BudgetMonthCategoryGroupViewModel>();
            CategoryGroupsCVS = new CollectionViewSource()
            {
                Source = CategoryGroups,
                IsSourceGrouped = true,
                ItemsPath = new PropertyPath(nameof(BudgetMonthCategoryGroupViewModel.Categories))
            };

            RefreshCategories();

            IsActive = true;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            Messenger.Register<BudgetMonthViewModel, BudgetedAmountChangedMessage>(this, (r, m) =>
            {
                r.DetailPropertiesChanged();
            });

            Messenger.Register<BudgetMonthViewModel, BudgetedAmountResetMessage>(this, (r, m) =>
            {
                r.DetailPropertiesChanged();
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

        public void RefreshCategories()
        {
            foreach (var category in CategoryGroups)
            {
                category.IsActive = false;
            }
            CategoryGroups.Clear();

            foreach (var categoryGroup in _budgetMonth.Budget.CategoryGroups)
            {
                CategoryGroups.Add(new BudgetMonthCategoryGroupViewModel(
                    _accountDetailService, _budgetService, _budgetMonth, categoryGroup));
            }

            DetailPropertiesChanged();
            OnPropertyChanged(nameof(TotalActivity));
        }

        [RelayCommand]
        public void CopyBudgetedAmountsFromLastMonth()
        {
            var lastBudgetMonth = GetLastBudgetMonth();
            if (lastBudgetMonth is null)
            {
                ZeroAllBudgetedAmounts();
                return;
            }

            foreach (var category in _budgetMonth.Budget.CategoryGroups.SelectMany(x => x.Categories))
            {
                var lastMonthBudgetedAmount = _budgetService.GetTotalBudgetedForMonth(lastBudgetMonth, category);
                if (_budgetService.GetTotalBudgetedForMonth(_budgetMonth, category) != lastMonthBudgetedAmount)
                {
                    _budgetService.SetBudgetedAmount(_budgetMonth, category, lastMonthBudgetedAmount);
                }
            }

            Messenger.Send(new BudgetedAmountResetMessage(_budgetMonth));
        }

        [RelayCommand]
        public void ZeroAllBudgetedAmounts()
        {
            foreach (var category in _budgetMonth.Budget.CategoryGroups.SelectMany(x => x.Categories))
            {
                if (_budgetService.GetTotalBudgetedForMonth(_budgetMonth, category) != decimal.Zero)
                {
                    _budgetService.SetBudgetedAmount(_budgetMonth, category, decimal.Zero);
                }
            }

            Messenger.Send(new BudgetedAmountResetMessage(_budgetMonth));
        }

        private BudgetMonth GetLastBudgetMonth()
        {
            (int lastMonth, int yearOfLastMonth) = DateTimeHelper.GetPreviousMonth(_budgetMonth.Month, _budgetMonth.Year);
            return _budgetService.GetBudgetMonth(lastMonth, yearOfLastMonth);
        }

        private void DetailPropertiesChanged()
        {
            OnPropertyChanged(nameof(TotalBudgeted));
            OnPropertyChanged(nameof(TotalBalance));
            OnPropertyChanged(nameof(UnbudgetedLastMonth));
            OnPropertyChanged(nameof(OverspentLastMonth));
            OnPropertyChanged(nameof(TotalUnbudgeted));
        }
    }
}
