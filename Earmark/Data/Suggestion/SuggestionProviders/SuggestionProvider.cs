using System;
using System.Collections.Generic;
using System.Linq;

namespace Earmark.Data.Suggestion.SuggestionProviders
{
    public class SuggestionProvider<TModel, TSuggestion> : ISuggestionProvider where TSuggestion : ISuggestion
    {
        private const int MaxNumberOfSuggestions = 50;
        private IEnumerable<TModel> _models;

        private Func<IEnumerable<TModel>> _modelsCallback;
        private Func<TModel, string> _modelNameGetter;
        private Func<TModel, object, TSuggestion> _suggestionCreator;
        private Func<CreateSuggestion, ISuggestion> _createSuggestionCallback;

        public SuggestionProvider(
            Func<IEnumerable<TModel>> modelsCallback,
            Func<TModel, string> modelNameGetter,
            Func<TModel, object, TSuggestion> suggestionCreator,
            Func<CreateSuggestion, ISuggestion> createSuggestionCallback = null)
        {
            _modelsCallback = modelsCallback;
            _modelNameGetter = modelNameGetter;
            _suggestionCreator = suggestionCreator;
            _createSuggestionCallback = createSuggestionCallback;
        }

        public void PrepareForQuery()
        {
            _models = _modelsCallback();
        }

        public IEnumerable<ISuggestion> GetSuggestionsByQuery(string query, object predicateArg)
        {
            var splitQuery = query.ToLower().Split(' ');
            var models = _models
                .Take(MaxNumberOfSuggestions)
                .OrderBy(x => _modelNameGetter(x))
                .Where(x => splitQuery.All(y => _modelNameGetter(x).ToLower().Contains(y)));

            foreach (var model in models)
            {
                var suggestion = _suggestionCreator(model, predicateArg);
                if (suggestion is not null)
                {
                    yield return suggestion;
                }
            }

            if (_createSuggestionCallback is not null &&
                !string.IsNullOrEmpty(query) &&
                !models.Select(x => _modelNameGetter(x)).Contains(query))
            {
                yield return new CreateSuggestion(query);
            }
        }

        public ISuggestion InvokeCreateSuggestionCallback(CreateSuggestion createSuggestion)
        {
            return _createSuggestionCallback?.Invoke(createSuggestion);
        }
    }
}
