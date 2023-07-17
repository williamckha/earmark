using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Earmark.Backend.Services;
using Earmark.Data.CreationSpecs;
using Earmark.Data.Navigation;
using Earmark.Helpers;
using Earmark.Services;
using Earmark.ViewModels.Budget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Earmark.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private IAccountService _accountService;
        private ICategoriesService _categoriesService;
        private IDialogService _dialogService;
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

        public List<INavigationItem> NavigationFooterItems { get; } = new()
        {
            new NavigationItem()
            {
                Name = "NavigationItem_Settings".GetLocalizedResource(),
                IconGlyph = "\uE713",
                TargetViewModel = typeof(ReportsViewModel)
            }
        };

        public MainViewModel(
            IAccountService accountService, 
            ICategoriesService categoriesService,
            IDialogService dialogService, 
            INavigationService navigationService)
        {
            _accountService = accountService;
            _categoriesService = categoriesService;
            _dialogService = dialogService;
            _navigationService = navigationService;

            RefreshAccountGroupNavigationItems();
            SelectedNavigationItem = NavigationItems.First();
        }

        [RelayCommand]
        public async Task OpenAddAccountDialog()
        {
            if (!_dialogService.IsDialogOpen)
            {
                var accountCreationSpec = await _dialogService.OpenAddAccountDialog();
                if (accountCreationSpec is not null)
                {
                    AddAccount(accountCreationSpec);
                }
            }
        }

        public void AddAccount(AccountCreationSpec accountCreationSpec)
        {
            var account = _accountService.AddAccount(accountCreationSpec.AccountName);

            // Add the starting balance transaction.
            var transaction = _accountService.AddTransaction(account.Id, DateTime.Now, accountCreationSpec.StartingBalance);
            _accountService.SetMemoForTransaction(transaction.Id, "StartingBalanceTransactionMemo".GetLocalizedResource());

            // Starting balance should be categorized as income.
            var incomeCategory = _categoriesService.GetCategories().FirstOrDefault(x => x.IsIncome);
            if (incomeCategory is not null)
            {
                _accountService.SetCategoryForTransaction(transaction.Id, incomeCategory.Id);
            }

            RefreshAccountGroupNavigationItems();
            SelectedNavigationItem = NavigationItems
                .OfType<AccountGroupNavigationItem>()
                .SelectMany(x => x.AccountNavigationItems)
                .FirstOrDefault(x => x.Id == account.Id);
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

            NavigationItems.Add(new AccountGroupNavigationItem(_accountService, _accountService.GetAccounts())
            {
                Name = "NavigationItem_Accounts".GetLocalizedResource(),
                AccountGroupName = "AccountGroupName_AllAccounts".GetLocalizedResource(),
                IconGlyph = "\uE825"
            });
        }
    }
}
