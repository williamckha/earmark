using Earmark.Backend.Models;
using System.Collections.Generic;

namespace Earmark.Backend.Services
{
    public interface ICategoriesService
    {
        /// <summary>
        /// Gets all category groups.
        /// </summary>
        /// <returns>All the category groups.</returns>
        IEnumerable<CategoryGroup> GetCategoryGroups();

        /// <summary>
        /// Gets all categories.
        /// </summary>
        /// <returns>All the categories.</returns>
        IEnumerable<Category> GetCategories();

        /// <summary>
        /// Adds a category group with the specified name.
        /// </summary>
        /// <param name="name">The name of the category group.</param>
        /// <returns>The added category group.</returns>
        CategoryGroup AddCategoryGroup(string name);

        /// <summary>
        /// Adds a category with the specified name.
        /// </summary>
        /// <param name="categoryGroup">The category group to put the category in.</param>
        /// <param name="name">The name of the category.</param>
        /// <returns>The added category.</returns>
        Category AddCategory(CategoryGroup categoryGroup, string name);

        /// <summary>
        /// Removes the category group and all of the categories in the group.
        /// </summary>
        /// <param name="categoryGroup">The category group to remove.</param>
        void RemoveCategoryGroup(CategoryGroup categoryGroup);

        /// <summary>
        /// Removes the category.
        /// </summary>
        /// <param name="category">The category to remove.</param>
        void RemoveCategory(Category category);
    }
}