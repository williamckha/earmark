using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Earmark.Backend.Models;
using Earmark.Data.Messages;
using Earmark.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Earmark.Data.Navigation
{
    public class AccountNavigationItem : ObservableRecipient, INavigationItem
    {
        private Account _account;

        public string Name => _account.Name;

        public string IconGlyph => null;

        public Type TargetViewModel => typeof(AccountViewModel);

        public object Parameter => new AccountGroup(Name, GetAccountIdAsSingleItemEnumerable());

        public decimal TotalBalance => _account.Transactions.Sum(x => x.Amount);

        public AccountNavigationItem(Account account) : base(StrongReferenceMessenger.Default)
        {
            _account = account;

            IsActive = true;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            Messenger.Register<AccountNavigationItem, AccountBalanceChangedMessage>(this, (r, m) =>
            {
                r.OnPropertyChanged(nameof(TotalBalance));
            });
        }

        private IEnumerable<Guid> GetAccountIdAsSingleItemEnumerable()
        {
            yield return _account.Id;
        }
    }
}
