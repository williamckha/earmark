using System;

namespace Earmark.Data.Suggestion
{
    public interface ISuggestion
    {
        Guid? Id { get; }

        string Name { get; }

        string QueryableName { get; }
    }
}
