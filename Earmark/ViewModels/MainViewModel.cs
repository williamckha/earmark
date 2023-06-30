using CommunityToolkit.Mvvm.ComponentModel;
using Earmark.Backend.Services;
using Earmark.Data.Navigation;
using Earmark.Helpers;
using Earmark.Services;
using Earmark.ViewModels.Budget;
using System.Collections.ObjectModel;
using System.Linq;

namespace Earmark.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private IAccountService _accountService;
        private INavigationService _navigationService;

        [ObservableProperty]
        private INavigationItem _selectedNavigationItem;

        public ObservableCollection<INavigationItem> NavigationItems { get; } = new()
        {
            new NavigationItem()
            {
                Name = "NavigationItem_Budget".GetLocalizedResource(),
                IconGlyph = "\uE8EC",
                TargetViewModel = typeof(BudgetViewModel)
            },
            new NavigationItem()
            {
                Name = "NavigationItem_Reports".GetLocalizedResource(),
                IconGlyph = "\uE9F9",
                TargetViewModel = typeof(ReportsViewModel)
            }
        };

        public MainViewModel(IAccountService accountService, INavigationService navigationService)
        {
            _accountService = accountService;
            _navigationService = navigationService;

            RefreshAccountGroupNavigationItems();
            SelectedNavigationItem = NavigationItems.First();
        }

        partial void OnSelectedNavigationItemChanged(INavigationItem value)
        {
            if (value is not null)
            {
                _navigationService.NavigateTo(value.TargetViewModel, value.Parameter);
            }
        }

        private void RefreshAccountGroupNavigationItems()
        {
            var accountGroups = NavigationItems.OfType<AccountGroupNavigationItem>().ToList();
            foreach (var accountGroup in accountGroups)
            {
                NavigationItems.Remove(accountGroup);
                accountGroup.IsActive = false;
            }

            NavigationItems.Add(new AccountGroupNavigationItem(_accountService.GetAccounts())
            {
                Name = "NavigationItem_Accounts".GetLocalizedResource(),
                AccountGroupName = "AccountGroupName_AllAccounts".GetLocalizedResource(),
                IconGlyph = "\uE825"
            });
        }
    }
}
