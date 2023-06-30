using Earmark.Backend.Services;
using System.Linq;

namespace Earmark.Helpers.Validation
{
    public class CategoryGroupValidator : IDataValidator<string>
    {
        private ICategoriesService _categoriesService;

        public CategoryGroupValidator(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        public bool Validate(string categoryGroupName, out string errorMessage)
        {
            bool categoryGroupExists = _categoriesService.GetCategoryGroups().Any(x => x.Name == categoryGroupName);
            errorMessage = categoryGroupExists ? "CategoryGroupAlreadyExistsErrorMessage".GetLocalizedResource() : string.Empty;
            return !categoryGroupExists;
        }
    }
}
