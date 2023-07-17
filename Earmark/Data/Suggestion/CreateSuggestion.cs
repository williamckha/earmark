using Earmark.Helpers;
using System;

namespace Earmark.Data.Suggestion
{
    public class CreateSuggestion : ISuggestion
    {
        public int? Id => null;

        public string Name => string.Format("CreateSuggestionText".GetLocalizedResource(), QueryableName);

        public string QueryableName { get; }

        public CreateSuggestion(string query)
        {
            QueryableName = query;
        }
    }
}
