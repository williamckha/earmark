using Earmark.Backend.Models;
using System;
using System.Collections.Generic;

namespace Earmark.Backend.Services
{
    public interface ICategoriesService
    {
        /// <summary>
        /// Gets all category groups in the database.
        /// </summary>
        /// <returns>All the category groups in the database.</returns>
        IEnumerable<CategoryGroup> GetCategoryGroups();

        /// <summary>
        /// Gets all categories in the database.
        /// </summary>
        /// <returns>All the categories in the database.</returns>
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
        /// <param name="categoryGroupId">The ID of the category group to put the category in.</param>
        /// <param name="name">The name of the category.</param>
        /// <returns>The added category.</returns>
        Category AddCategory(int categoryGroupId, string name);

        /// <summary>
        /// Removes the category group and all of the categories in the group.
        /// </summary>
        /// <param name="categoryGroupId">The ID of the category group to remove.</param>
        void RemoveCategoryGroup(int categoryGroupId);

        /// <summary>
        /// Removes the category.
        /// </summary>
        /// <param name="categoryId">The ID of the category to remove.</param>
        void RemoveCategory(int categoryId);
    }
}