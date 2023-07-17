using System;

namespace Earmark.Data.Suggestion
{
    public interface ISuggestion
    {
        int? Id { get; }

        string Name { get; }

        string QueryableName { get; }
    }
}
