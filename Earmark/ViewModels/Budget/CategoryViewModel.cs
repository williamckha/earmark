using CommunityToolkit.Mvvm.ComponentModel;
using Earmark.Backend.Models;
using System;

namespace Earmark.ViewModels.Budget
{
    public partial class CategoryViewModel : ObservableObject
    {
        private Category _category;

        /// <summary>
        /// The name of the category.
        /// </summary>
        [ObservableProperty]
        private string _name;

        /// <summary>
        /// The ID that identifies the category.
        /// </summary>
        public Guid Id => _category.Id;

        /// <summary>
        /// The category group which contains the category.
        /// </summary>
        public CategoryGroupViewModel Group { get; set; }

        public CategoryViewModel(Category category, CategoryGroupViewModel group)
        {
            _category = category;

            Name = _category.Name;
            Group = group;
        }
    }
}
