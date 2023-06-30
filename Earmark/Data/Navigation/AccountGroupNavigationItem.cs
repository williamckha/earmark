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
    public class AccountGroupNavigationItem : ObservableRecipient, INavigationItem
    {
        private List<Account> _accounts;

        public string Name { get; init; }

        public string AccountGroupName { get; init; }

        public string IconGlyph { get; init; }

        public Type TargetViewModel => typeof(AccountViewModel);

        public object Parameter => new AccountGroup(AccountGroupName, _accounts.Select(x => x.Id));

        public decimal TotalBalance => _accounts.SelectMany(x => x.Transactions).Sum(x => x.Amount);

        public List<AccountNavigationItem> AccountNavigationItems { get; }

        public AccountGroupNavigationItem(IEnumerable<Account> accounts) : base(StrongReferenceMessenger.Default)
        {
            _accounts = accounts.ToList();

            AccountNavigationItems = new List<AccountNavigationItem>();
            foreach (var account in _accounts)
            {
                AccountNavigationItems.Add(new AccountNavigationItem(account));
            }

            IsActive = true;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
           
            Messenger.Register<AccountGroupNavigationItem, AccountBalanceChangedMessage>(this, (r, m) =>
            {
                r.OnPropertyChanged(nameof(TotalBalance));
            });
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            foreach (var accountNavigationItem in AccountNavigationItems)
            {
                accountNavigationItem.IsActive = false;
            }
        }
    }
}
