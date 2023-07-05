using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Earmark.Backend.Helpers;
using Earmark.Backend.Services;
using Earmark.Data.Messages;
using Earmark.Helpers.Validation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Earmark.ViewModels.Budget
{
    public partial class BudgetViewModel : ObservableRecipient
    {
        private IBudgetService _budgetService;
        private ICategoriesService _categoriesService;

        /// <summary>
        /// The currently displayed months in the budget.
        /// </summary>
        public ObservableCollection<BudgetMonthViewModel> Months { get; }

        /// <summary>
        /// The category groups in the budget.
        /// </summary>
        public ObservableCollection<CategoryGroupViewModel> CategoryGroups { get; }

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
            IBudgetService budgetService, 
            ICategoriesService categoriesService) : base(StrongReferenceMessenger.Default)
        {
            _budgetService = budgetService;
            _categoriesService = categoriesService;

            CategoryGroupValidator = new CategoryGroupValidator(_categoriesService);
            CategoryValidator = new CategoryValidator(_categoriesService);

            CategoryGroups = new ObservableCollection<CategoryGroupViewModel>();
            CategoryGroupsCVS = new CollectionViewSource()
            {
                Source = CategoryGroups,
                IsSourceGrouped = true,
                ItemsPath = new PropertyPath(nameof(CategoryGroupViewModel.Categories))
            };

            foreach (var categoryGroup in _categoriesService.GetCategoryGroups())
            {
                CategoryGroups.Add(new CategoryGroupViewModel(_categoriesService, _budgetService, CategoryValidator, categoryGroup));
            }

            Months = new ObservableCollection<BudgetMonthViewModel>();
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            foreach (var budgetMonth in Months)
            {
                budgetMonth.IsActive = false;
            }

            foreach (var categoryGroup in CategoryGroups)
            {
                categoryGroup.IsActive = false;
            }
        }

        [RelayCommand]
        public void AddCategoryGroup(string categoryGroupName)
        {
            var categoryGroup = _categoriesService.AddCategoryGroup(categoryGroupName);
            CategoryGroups.Add(new CategoryGroupViewModel(_categoriesService, _budgetService, CategoryValidator, categoryGroup));

            Messenger.Send(new CategoryGroupAddedMessage(categoryGroup));
        }

        [RelayCommand]
        public void RemoveCategoryGroup(CategoryGroupViewModel categoryGroupViewModel)
        {
            _categoriesService.RemoveCategoryGroup(categoryGroupViewModel.Id);
            _budgetService.UpdateTotalUnbudgetedAmounts();

            categoryGroupViewModel.IsActive = false;
            CategoryGroups.Remove(categoryGroupViewModel);

            Messenger.Send(new CategoryGroupRemovedMessage(categoryGroupViewModel.Id));
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
                var currentDate = DateTime.Now;
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

                Months.Add(new BudgetMonthViewModel(_budgetService, _categoriesService, budgetMonth));

                (month, year) = DateTimeHelper.GetNextMonth(month, year);
            }
        }
    }
}
