using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.UI;
using Earmark.Backend.Models;
using Earmark.Backend.Services;
using Earmark.Backend.Services.TransactionImporter;
using Earmark.Data.Messages;
using Earmark.Data.Navigation;
using Earmark.Data.Suggestion;
using Earmark.Data.Suggestion.SuggestionProviders;
using Earmark.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Earmark.ViewModels.Account
{
    public partial class AccountViewModel : ObservableRecipient
    {
        private IAccountService _accountService;
        private IBudgetService _budgetService;
        private IPayeeService _payeeService;
        private ICategoriesService _categoriesService;

        [ObservableProperty]
        private string _accountName;

        [ObservableProperty]
        private int _totalBalance;

        public IEnumerable<int> AccountIds { get; private set; }

        public bool IsMultipleAccounts => AccountIds.Count() > 1;

        public SuggestionProvider<Backend.Models.Account, AccountSuggestion> AccountSuggestionProvider { get; }

        public SuggestionProvider<Payee, PayeeSuggestion> PayeeSuggestionProvider { get; }

        public SuggestionProvider<Category, CategorySuggestion> CategorySuggestionProvider { get; }

        public ObservableCollection<TransactionViewModel> Transactions { get; }

        public AdvancedCollectionView TransactionsACV { get; }

        public AccountViewModel(
            IAccountService accountService,
            IBudgetService budgetService,
            IPayeeService payeeService,
            ICategoriesService categoriesService) : base(StrongReferenceMessenger.Default)
        {
            _accountService = accountService;
            _budgetService = budgetService;
            _payeeService = payeeService;
            _categoriesService = categoriesService;

            Transactions = new ObservableCollection<TransactionViewModel>();
            TransactionsACV = new AdvancedCollectionView(Transactions, true);
            TransactionsACV.SortDescriptions.Add(new SortDescription(nameof(TransactionViewModel.Date), SortDirection.Descending));

            AccountSuggestionProvider = new(
                () => _accountService.GetAccounts(),
                model => model.Name,
                (model, predicateArg) =>
                {
                    // The account and payee for a transaction must be different.
                    // Ensure that we don't return account suggestions that match the payee for the transaction.
                    if (predicateArg is TransactionViewModel transactionViewModel &&
                        transactionViewModel.ChosenPayeeSuggestion.Id == model.TransferPayee.Id)
                    {
                        return null;
                    }

                    return new AccountSuggestion(model);
                });

            PayeeSuggestionProvider = new(
                () => _payeeService.GetPayees(),
                model => model.Name,
                (model, predicateArg) =>
                {
                    // The account and payee for a transaction must be different.
                    // Ensure that we don't return payee suggestions that match the account for the transaction.
                    if (predicateArg is TransactionViewModel transactionViewModel &&
                        transactionViewModel.ChosenAccountSuggestion.Id == model.TransferAccount?.Id)
                    {
                        return null;
                    }

                    return new PayeeSuggestion(model);
                },
                createSuggestion =>
                {
                    var payee =
                        _payeeService.GetPayees().FirstOrDefault(x => x.Name == createSuggestion.QueryableName) ??
                        _payeeService.AddPayee(createSuggestion.QueryableName);

                    return new PayeeSuggestion(payee);
                });

            CategorySuggestionProvider = new(
                () => _categoriesService.GetCategories(),
                model => model.Name,
                (model, _) => new CategorySuggestion(model));

            IsActive = true;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            Messenger.Register<AccountViewModel, AccountBalanceChangedMessage>(this, (r, m) =>
            {
                r.TotalBalance = r._accountService.GetTotalBalanceForAccounts(r.AccountIds);
            });

            Messenger.Register<AccountViewModel, TransferTransactionChangedMessage>(this, (r, m) =>
            {
                if (m.OldTransferTransaction is not null)
                {
                    m.OldTransferTransaction.IsActive = false;
                    r.Transactions.Remove(m.OldTransferTransaction);
                }

                if (m.NewTransferTransaction is not null &&
                    r.AccountIds.Contains(m.NewTransferTransaction.Account.Id))
                {
                    r.AddTransactionViewModel(m.NewTransferTransaction);
                }
            });
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            foreach (var transaction in Transactions)
            {
                transaction.IsActive = false;
            }
        }

        public void LoadAccounts(AccountGroup accountGroup)
        {
            AccountIds = accountGroup.AccountIds.ToList();
            AccountName = accountGroup.Name;

            var transactions = _accountService.GetTransactions(AccountIds);
            foreach (var transaction in transactions)
            {
                AddTransactionViewModel(transaction);
            }

            Messenger.Send(new AccountBalanceChangedMessage());
        }

        [RelayCommand]
        public void AddTransaction()
        {
            if (AccountIds.Any())
            {
                var transaction = _accountService.AddTransaction(AccountIds.First(), DateTime.Now, 0);
                AddTransactionViewModel(transaction);

                Messenger.Send(new AccountBalanceChangedMessage());
            }
        }

        [RelayCommand]
        public void RemoveTransaction(TransactionViewModel transactionViewModel)
        {
            _accountService.RemoveTransaction(transactionViewModel.Id);
            if (transactionViewModel.TransferTransactionId is int transferTransactionId)
            {
                _accountService.RemoveTransaction(transferTransactionId);
            }

            _budgetService.UpdateTotalUnbudgetedAmounts(transactionViewModel.Date.Month, transactionViewModel.Date.Year);

            var transferTransaction = transactionViewModel.GetTransferTransactionViewModel();
            if (transferTransaction is not null)
            {
                transferTransaction.IsActive = false;
                Transactions.Remove(transferTransaction);
            }

            transactionViewModel.IsActive = false;
            Transactions.Remove(transactionViewModel);

            Messenger.Send(new AccountBalanceChangedMessage());
        }

        [RelayCommand]
        public async Task ImportTransactions()
        {
            var file = await FilePickerHelper.PickSingleFile(new string[] { ".csv" });
            if (file is not null)
            {
                var transactionImporter = new CsvTransactionImporter(_accountService, _payeeService);
                var importedTransactions = transactionImporter.ImportTransactionsFromFile(file.Path);
                foreach (var transaction in importedTransactions)
                {
                    AddTransactionViewModel(transaction);
                }
            }

            Messenger.Send(new AccountBalanceChangedMessage());
        }

        private void AddTransactionViewModel(Transaction transaction)
        {
            var transactionViewModel = new TransactionViewModel(
                _accountService, _budgetService, _payeeService, _categoriesService, transaction);

            Transactions.Add(transactionViewModel);
        }
    }
}
