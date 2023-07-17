using Earmark.Backend.Models;
using System;

namespace Earmark.Data.Suggestion
{
    public class PayeeSuggestion : ISuggestion
    {
        private Payee _payee;

        public int? Id => _payee?.Id;

        public string Name => _payee?.Name ?? string.Empty;

        public string QueryableName => _payee?.Name ?? string.Empty;

        public PayeeSuggestion(Payee payee)
        {
            _payee = payee;
        }
    }
}
