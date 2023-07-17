using System;

namespace Earmark.Data.Suggestion
{
    public class AccountSuggestion : ISuggestion
    {
        private Backend.Models.Account _account;

        public int? Id => _account.Id;

        public string Name => _account.Name;

        public string QueryableName => _account.Name;

        public AccountSuggestion(Backend.Models.Account account)
        {
            _account = account;
        }
    }
}
