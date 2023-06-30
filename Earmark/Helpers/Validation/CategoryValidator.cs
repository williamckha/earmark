using Earmark.Backend.Services;
using System.Linq;

namespace Earmark.Helpers.Validation
{
    public class CategoryValidator : IDataValidator<string>
    {
        private ICategoriesService _categoriesService;

        public CategoryValidator(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        public bool Validate(string categoryName, out string errorMessage)
        {
            bool categoryExists = _categoriesService.GetCategories().Any(x => x.Name == categoryName);
            errorMessage = categoryExists ? "CategoryAlreadyExistsErrorMessage".GetLocalizedResource() : string.Empty;
            return !categoryExists;
        }
    }
}
