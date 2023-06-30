using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Earmark.Backend.Helpers;
using Earmark.Backend.Services;
using Earmark.Helpers;
using Earmark.Helpers.Validation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Earmark.ViewModels.Budget
{
    public partial class BudgetViewModel : ObservableRecipient
    {
        private IAccountDetailService _accountDetailService;
        private IBudgetService _budgetService;
        private ICategoriesService _categoriesService;

        private Backend.Models.Budget _budget;

        /// <summary>
        /// The currently displayed months in the budget.
        /// </summary>
        public ObservableCollection<BudgetMonthViewModel> Months { get; }

        /// <summary>
        /// The category groups in the budget.
        /// </summary>
        public ObservableGroupingCollection<CategoryGroupViewModel> CategoryGroups { get; }

        /// <summary>
        /// The collection view source that adds grouping support to CategoryGroups.
        /// </summary>
        public CollectionViewSource CategoryGroupsCVS { get; }

        /// <summary>
        /// The data validator for validating category group names.
        /// </summary>
        public CategoryGroupValidator CategoryGroupValidator { get; }

        /// <summary>
        /// The data validator for validating category names.
        /// </summary>
        public CategoryValidator CategoryValidator { get; }

        public BudgetViewModel(
            IAccountDetailService accountDetailService,
            IBudgetService budgetService, 
            ICategoriesService categoriesService) : base(StrongReferenceMessenger.Default)
        {
            _accountDetailService = accountDetailService;
            _budgetService = budgetService;
            _categoriesService = categoriesService;

            _budget = _budgetService.GetBudget();

            CategoryGroupValidator = new CategoryGroupValidator(_categoriesService);
            CategoryValidator = new CategoryValidator(_categoriesService);

            CategoryGroups = new ObservableGroupingCollection<CategoryGroupViewModel>();
            CategoryGroupsCVS = new CollectionViewSource()
            {
                Source = CategoryGroups,
                IsSourceGrouped = true,
                ItemsPath = new PropertyPath(nameof(CategoryGroupViewModel.Categories))
            };

            foreach (var categoryGroup in _budget.CategoryGroups)
            {
                CategoryGroups.Add(new CategoryGroupViewModel(_categoriesService, CategoryValidator, categoryGroup));
            }

            CategoryGroups.CollectionChanged += CategoryGroups_CollectionChanged;
            CategoryGroups.GroupingChanged += CategoryGroups_GroupingChanged;

            Months = new ObservableCollection<BudgetMonthViewModel>();
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            foreach (var budgetMonth in Months)
            {
                budgetMonth.IsActive = false;
            }
        }

        [RelayCommand]
        public void AddCategoryGroup(string categoryGroupName)
        {
            var categoryGroup = _categoriesService.AddCategoryGroup(categoryGroupName);
            CategoryGroups.Add(new CategoryGroupViewModel(_categoriesService, CategoryValidator, categoryGroup));
        }

        [RelayCommand]
        public void RemoveCategoryGroup(CategoryGroupViewModel categoryGroupViewModel)
        {
            var categoryGroup = _budget.CategoryGroups.First(x => x.Id == categoryGroupViewModel.Id);
            _categoriesService.RemoveCategoryGroup(categoryGroup);
            CategoryGroups.Remove(categoryGroupViewModel);
        }

        [RelayCommand]
        public void NavigateToPreviousMonth()
        {
            var currentMonth = Months.First();
            var numberOfMonths = Months.Count();
            (int previousMonth, int yearOfPreviousMonth) =
                DateTimeHelper.GetPreviousMonth(currentMonth.Month, currentMonth.Year);
            NavigateToMonth(previousMonth, yearOfPreviousMonth, numberOfMonths);
        }

        [RelayCommand]
        public void NavigateToNextMonth()
        {
            var currentMonth = Months.First();
            var numberOfMonths = Months.Count();
            (int nextMonth, int yearOfNextMonth) =
                DateTimeHelper.GetNextMonth(currentMonth.Month, currentMonth.Year);
            NavigateToMonth(nextMonth, yearOfNextMonth, numberOfMonths);
        }

        [RelayCommand]
        public void RefreshMonths(int numberOfMonths)
        {
            if (numberOfMonths != Months.Count())
            {
                var currentDate = DateTimeOffset.Now;
                var firstMonth = Months.FirstOrDefault();
                (int month, int year) = firstMonth is not null ?
                    (firstMonth.Month, firstMonth.Year) :
                    (currentDate.Month, currentDate.Year);

                NavigateToMonth(month, year, numberOfMonths);
            }
        }

        private void NavigateToMonth(int month, int year, int numberOfMonths)
        {
            foreach (var budgetMonth in Months)
            {
                budgetMonth.IsActive = false;
            }
            Months.Clear();

            for (int i = 0; i < numberOfMonths; i++)
            {
                var budgetMonth = 
                    _budgetService.GetBudgetMonth(month, year) ??
                    _budgetService.AddBudgetMonth(month, year);

                Months.Add(new BudgetMonthViewModel(
                    _accountDetailService, _budgetService, budgetMonth));

                (month, year) = DateTimeHelper.GetNextMonth(month, year);
            }
        }

        private void RefreshCategoriesInMonths()
        {
            foreach (var month in Months)
            {
                month.RefreshCategories();
            }
        }

        private void CategoryGroups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshCategoriesInMonths();
        }

        private void CategoryGroups_GroupingChanged(object sender, EventArgs e)
        {
            RefreshCategoriesInMonths();
        }
    }
}
