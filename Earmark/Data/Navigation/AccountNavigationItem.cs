using CommunityToolkit.Mvvm.ComponentModel;
using Earmark.Backend.Models;
using Earmark.ViewModels.Account;
using System;
using System.Collections.Generic;

namespace Earmark.Data.Navigation
{
    public partial class AccountNavigationItem : ObservableObject, INavigationItem
    {
        [ObservableProperty]
        private int _totalBalance;

        public int Id { get; }

        public string Name { get; }

        public string IconGlyph => null;

        public Type TargetViewModel => typeof(AccountViewModel);

        public object Parameter => new AccountGroup(Name, GetAccountIdAsSingleItemEnumerable());

        public AccountNavigationItem(Account account)
        {
            Id = account.Id;
            Name = account.Name;
        }

        private IEnumerable<int> GetAccountIdAsSingleItemEnumerable()
        {
            yield return Id;
        }
    }
}
