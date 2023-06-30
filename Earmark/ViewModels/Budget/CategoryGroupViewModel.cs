using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Earmark.Backend.Models;
using Earmark.Backend.Services;
using Earmark.Helpers;
using Earmark.Helpers.Validation;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Earmark.ViewModels.Budget
{
    public partial class CategoryGroupViewModel : ObservableObject, IObservableGrouping
    {
        private ICategoriesService _categoriesService;

        private CategoryGroup _categoryGroup;

        public event EventHandler GroupingChanged;

        /// <summary>
        /// The name of the category group.
        /// </summary>
        [ObservableProperty]
        private string _name;

        /// <summary>
        /// The ID which identifies the grouping.
        /// </summary>
        public Guid Id => _categoryGroup.Id;

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
            CategoryValidator categoryValidator, 
            CategoryGroup categoryGroup)
        {
            _categoriesService = categoriesService;

            _categoryGroup = categoryGroup;
            
            Name = _categoryGroup.Name;
            CategoryValidator = categoryValidator;

            Categories = new ObservableCollection<CategoryViewModel>();
            Categories.CollectionChanged += Categories_CollectionChanged;
            foreach (var category in _categoryGroup.Categories)
            {
                Categories.Add(new CategoryViewModel(category, this));
            }
        }

        [RelayCommand]
        public void AddCategory(string categoryName)
        {
            var category = _categoriesService.AddCategory(_categoryGroup, categoryName);
            Categories.Add(new CategoryViewModel(category, this));
        }

        [RelayCommand]
        public void RemoveCategory(CategoryViewModel categoryViewModel)
        {
            var category = _categoryGroup.Categories.First(x => x.Id == categoryViewModel.Id);
            _categoriesService.RemoveCategory(category);
            Categories.Remove(categoryViewModel);
        }

        private void Categories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            GroupingChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
