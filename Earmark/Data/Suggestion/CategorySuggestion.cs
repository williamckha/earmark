using Earmark.Backend.Models;
using System;

namespace Earmark.Data.Suggestion
{
    public class CategorySuggestion : ISuggestion
    {
        private Category _category;

        public int? Id => _category?.Id;

        public string Name => _category?.Name ?? string.Empty;

        public string QueryableName => _category?.Name ?? string.Empty;

        public CategorySuggestion(Category category)
        {
            _category = category;
        }
    }
}
