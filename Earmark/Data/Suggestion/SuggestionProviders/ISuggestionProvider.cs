using System.Collections.Generic;

namespace Earmark.Data.Suggestion.SuggestionProviders
{
    public interface ISuggestionProvider
    {
        /// <summary>
        /// Prepares for a sequence of queries for suggestions by refreshing the data backing the SuggestionProvider.
        /// This ensures that the suggestions provided by the SuggestionProvider are up to date with the data model.
        /// </summary>
        void PrepareForQuery();

        /// <summary>
        /// Generates suggestions based on a provided query and a specified object to compare against a predicate.
        /// The predicate is internal to the ISuggestionProvider and must pass for a given suggestion to be returned.
        /// </summary>
        /// <param name="query">The query to use when generating suggestions.</param>
        /// <param name="predicateArg">The object to compare against a predicate supplied by the ISuggestionProvider.</param>
        /// <returns>The generated suggestions based on the provided query and predicate argument.</returns>
        IEnumerable<ISuggestion> GetSuggestionsByQuery(string query, object predicateArg);

        /// <summary>
        /// Invokes the registered callback (if one exists) that returns a new suggestion from a CreateSuggestion.
        /// </summary>
        /// <param name="createSuggestion">The CreateSuggestion to pass into the callback.</param>
        /// <returns>The suggestion returned by the callback.</returns>
        ISuggestion InvokeCreateSuggestionCallback(CreateSuggestion createSuggestion);
    }
}
