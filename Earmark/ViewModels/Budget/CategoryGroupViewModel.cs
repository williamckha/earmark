using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Earmark.Backend.Models;
using Earmark.Backend.Services;
using Earmark.Data.Messages;
using Earmark.Helpers.Validation;
using System;
using System.Collections.ObjectModel;

namespace Earmark.ViewModels.Budget
{
    public partial class CategoryGroupViewModel : ObservableRecipient
    {
        private ICategoriesService _categoriesService;
        private IBudgetService _budgetService;

        /// <summary>
        /// The ID which identifies the category group.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The name of the category group.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The categories contained in the category group.
        /// </summary>
        public ObservableCollection<CategoryViewModel> Categories { get; }

        /// <summary>
        /// The data validator for validating category names.
        /// </summary>
        public CategoryValidator CategoryValidator { get; }

        public CategoryGroupViewModel(
            ICategoriesService categoriesService, 
            IBudgetService budgetService,
            CategoryValidator categoryValidator, 
            CategoryGroup categoryGroup) : base(StrongReferenceMessenger.Default)
        {
            _categoriesService = categoriesService;
            _budgetService = budgetService;

            Id = categoryGroup.Id;
            Name = categoryGroup.Name;

            CategoryValidator = categoryValidator;

            Categories = new ObservableCollection<CategoryViewModel>();
            foreach (var category in categoryGroup.Categories)
            {
                Categories.Add(new CategoryViewModel(category));
            }

            IsActive = true;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            Messenger.Register<CategoryGroupViewModel, CategoryGroupViewModelRequestMessage, int>(this, Id, (r, m) => m.Reply(r));
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            foreach (var category in Categories)
            {
                category.IsActive = false;
            }
        }

        [RelayCommand]
        public void AddCategory(string categoryName)
        {
            var category = _categoriesService.AddCategory(Id, categoryName);
            Categories.Add(new CategoryViewModel(category));

            Messenger.Send(new CategoryAddedMessage(category), Id);
        }

        [RelayCommand]
        public void RemoveCategory(CategoryViewModel categoryViewModel)
        {
            _categoriesService.RemoveCategory(categoryViewModel.Id);
            _budgetService.UpdateTotalUnbudgetedAmounts();

            Categories.Remove(categoryViewModel);

            Messenger.Send(new CategoryRemovedMessage(categoryViewModel.Id), Id);
            Messenger.Send(new CategoryRemovedMessage(categoryViewModel.Id), nameof(BudgetMonthViewModel));
        }
    }
}
