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
        private IPayeeService _payeeService;
        private ICategoriesService _categoriesService;

        private string _accountGroupName;
        private IEnumerable<Backend.Models.Account> _accounts;

        public string AccountName
        {
            get => IsMultipleAccounts ? _accountGroupName : _accounts.FirstOrDefault()?.Name;
            set
            {
                // Account renaming to be implemented.
                throw new NotImplementedException();
            }
        }

        public bool IsMultipleAccounts => _accounts.Count() > 1;

        public decimal TotalBalance => _accounts.SelectMany(x => x.Transactions).Sum(x => x.Amount);

        public SuggestionProvider<Backend.Models.Account, AccountSuggestion> AccountSuggestionProvider { get; }

        public SuggestionProvider<Payee, PayeeSuggestion> PayeeSuggestionProvider { get; }

        public SuggestionProvider<Category, CategorySuggestion> CategorySuggestionProvider { get; }

        public ObservableCollection<TransactionViewModel> Transactions { get; }

        public AdvancedCollectionView TransactionsACV { get; }

        public AccountViewModel(
            IAccountService accountService,
            IPayeeService payeeService,
            ICategoriesService categoriesService) : base(StrongReferenceMessenger.Default)
        {
            _accountService = accountService;
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
                r.OnPropertyChanged(nameof(TotalBalance));
            });

            Messenger.Register<AccountViewModel, TransactionViewModelRequestMessage>(this, (r, m) =>
            {
                if (m.Transaction is not null)
                {
                    m.Reply(r.Transactions.FirstOrDefault(x => x.Id == m.Transaction.Id));
                }
                else
                {
                    m.Reply(null);
                }
                
            });

            Messenger.Register<AccountViewModel, TransferTransactionChangedMessage>(this, (r, m) =>
            {
                if (m.OldTransferTransaction is not null)
                {
                    var oldTransferTransaction = r.Transactions.FirstOrDefault(x => x.Id == m.OldTransferTransaction.Id);
                    if (oldTransferTransaction is not null)
                    {
                        r.Transactions.Remove(oldTransferTransaction);
                        oldTransferTransaction.IsActive = false;
                    }
                }

                if (m.NewTransferTransaction is not null &&
                    r._accounts.Contains(m.NewTransferTransaction.Account))
                {
                    r.AddTransactionViewModel(m.NewTransferTransaction);
                }

                r.Messenger.Send(new AccountBalanceChangedMessage());
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
            _accountGroupName = accountGroup.Name;
            _accounts = _accountService
                .GetAccounts()
                .IntersectBy(accountGroup.AccountIds, x => x.Id)
                .ToList();

            foreach (var transaction in _accounts.SelectMany(x => x.Transactions))
            {
                AddTransactionViewModel(transaction);
            }
        }

        [RelayCommand]
        public void AddTransaction()
        {
            if (!_accounts.Any()) return;

            var transaction = _accountService.AddTransaction(_accounts.First(), DateTimeOffset.Now, decimal.Zero);
            AddTransactionViewModel(transaction);

            Messenger.Send(new AccountBalanceChangedMessage());
        }

        [RelayCommand]
        public void RemoveTransaction(TransactionViewModel transactionViewModel)
        {
            Transactions.Remove(transactionViewModel);
            transactionViewModel.IsActive = false;

            var transferTransaction = transactionViewModel.TransferTransaction;
            if (transferTransaction is not null)
            {
                Transactions.Remove(transferTransaction);
                transferTransaction.IsActive = false;
            }

            var transaction = _accounts.SelectMany(x => x.Transactions).First(x => x.Id == transactionViewModel.Id);
            _accountService.RemoveTransaction(transaction);

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

        private TransactionViewModel AddTransactionViewModel(Transaction transaction)
        {
            var transactionViewModel = new TransactionViewModel(_accountService, _payeeService, _categoriesService, transaction);
            Transactions.Add(transactionViewModel);

            return transactionViewModel;
        }
    }
}
