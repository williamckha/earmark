using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Earmark.Backend.Models;
using Earmark.Backend.Services;
using Earmark.Data.Messages;
using Earmark.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Earmark.Data.Navigation
{
    public partial class AccountGroupNavigationItem : ObservableRecipient, INavigationItem
    {
        private IAccountService _accountService;

        [ObservableProperty]
        private int _totalBalance;

        public string Name { get; init; }

        public string AccountGroupName { get; init; }

        public string IconGlyph { get; init; }

        public Type TargetViewModel => typeof(AccountViewModel);

        public object Parameter => new AccountGroup(AccountGroupName, AccountNavigationItems.Select(x => x.Id));

        public List<AccountNavigationItem> AccountNavigationItems { get; }

        public AccountGroupNavigationItem(IAccountService accountService, IEnumerable<Account> accounts) : base(StrongReferenceMessenger.Default)
        {
            _accountService = accountService;

            AccountNavigationItems = new List<AccountNavigationItem>();
            foreach (var account in accounts)
            {
                AccountNavigationItems.Add(new AccountNavigationItem(account));
            }

            UpdateTotalBalances();

            IsActive = true;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
           
            Messenger.Register<AccountGroupNavigationItem, AccountBalanceChangedMessage>(this, (r, m) => r.UpdateTotalBalances());
        }

        private void UpdateTotalBalances()
        {
            var accounts = _accountService.GetAccounts();
            foreach (var accountNavigationItem in AccountNavigationItems)
            {
                var account = accounts.First(x => x.Id == accountNavigationItem.Id);
                accountNavigationItem.TotalBalance = account.TotalBalance;
            }

            TotalBalance = AccountNavigationItems.Sum(x => x.TotalBalance);
        }
    }
}
