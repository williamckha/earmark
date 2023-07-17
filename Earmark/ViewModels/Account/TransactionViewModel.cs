using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Earmark.Backend.Models;
using Earmark.Backend.Services;
using Earmark.Data.Messages;
using Earmark.Data.Suggestion;
using System;
using System.Linq;

namespace Earmark.ViewModels.Account
{
    public partial class TransactionViewModel : ObservableRecipient
    {
        private IAccountService _accountService;
        private IBudgetService _budgetService;
        private IPayeeService _payeeService;
        private ICategoriesService _categoriesService;

        [ObservableProperty]
        private DateTime _date;

        [ObservableProperty]
        private string _memo;

        [ObservableProperty]
        private int _amount;

        [ObservableProperty]
        private AccountSuggestion _chosenAccountSuggestion;

        [ObservableProperty]
        private PayeeSuggestion _chosenPayeeSuggestion;
        
        [ObservableProperty]
        private CategorySuggestion _chosenCategorySuggestion;

        public int Id { get; }

        public int? TransferTransactionId { get; private set; }

        public bool IsTransfer => TransferTransactionId is not null;

        public TransactionViewModel(
            IAccountService accountService,
            IBudgetService budgetService,
            IPayeeService payeeService,
            ICategoriesService categoriesService,
            Transaction transaction) : base(StrongReferenceMessenger.Default)
        {
            _accountService = accountService;
            _budgetService = budgetService;
            _payeeService = payeeService;
            _categoriesService = categoriesService;

            Id = transaction.Id;
            TransferTransactionId = transaction.TransferTransaction?.Id;

            _date = transaction.Date;
            _memo = transaction.Memo;
            _amount = transaction.Amount;

            _chosenAccountSuggestion = new AccountSuggestion(transaction.Account);
            _chosenPayeeSuggestion = new PayeeSuggestion(transaction.Payee);
            _chosenCategorySuggestion = new CategorySuggestion(transaction.Category);

            IsActive = true;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            Messenger.Register<TransactionViewModel, TransactionViewModelRequestMessage, int>(this, Id, (r, m) => m.Reply(r));
        }

        partial void OnDateChanged(DateTime oldValue, DateTime newValue)
        {
            if (oldValue != newValue)
            {
                _accountService.SetDateForTransaction(Id, newValue);
                _budgetService.UpdateTotalUnbudgetedAmounts(Date.Month, Date.Year);

                var transferTransactionViewModel = GetTransferTransactionViewModel();
                if (transferTransactionViewModel is not null)
                {
                    transferTransactionViewModel._date = newValue;
                    transferTransactionViewModel.OnPropertyChanged(nameof(Date));
                }
            }
        }

        partial void OnMemoChanged(string oldValue, string newValue)
        {
            if (oldValue != newValue)
            {
                _accountService.SetMemoForTransaction(Id, newValue);

                var transferTransactionViewModel = GetTransferTransactionViewModel();
                if (transferTransactionViewModel is not null)
                {
                    transferTransactionViewModel._memo = newValue;
                    transferTransactionViewModel.OnPropertyChanged(nameof(Memo));
                }
            }
        }

        partial void OnAmountChanged(int oldValue, int newValue)
        {
            if (oldValue != newValue)
            {
                _accountService.SetAmountForTransaction(Id, newValue);
                _budgetService.UpdateTotalUnbudgetedAmounts(Date.Month, Date.Year);

                var transferTransactionViewModel = GetTransferTransactionViewModel();
                if (transferTransactionViewModel is not null)
                {
                    transferTransactionViewModel._amount = -newValue;
                    transferTransactionViewModel.OnPropertyChanged(nameof(Amount));
                }

                Messenger.Send(new AccountBalanceChangedMessage());
            }
        }

        partial void OnChosenAccountSuggestionChanged(AccountSuggestion oldValue, AccountSuggestion newValue)
        {
            if (oldValue.Id != newValue.Id)
            {
                _accountService.SetAccountForTransaction(Id, (int)newValue.Id);

                var transferTransactionViewModel = GetTransferTransactionViewModel();
                if (transferTransactionViewModel is not null)
                {
                    var transferPayee = _payeeService.GetPayees().First(x => x.TransferAccount.Id == newValue.Id);
                    transferTransactionViewModel._chosenPayeeSuggestion = new PayeeSuggestion(transferPayee);
                    transferTransactionViewModel.OnPropertyChanged(nameof(ChosenPayeeSuggestion));
                }

                Messenger.Send(new AccountBalanceChangedMessage());
            }
        }

        partial void OnChosenPayeeSuggestionChanged(PayeeSuggestion oldValue, PayeeSuggestion newValue)
        {
            if (oldValue.Id != newValue.Id)
            {
                var transaction = _accountService.SetPayeeForTransaction(Id, newValue.Id);

                Messenger.Send(new TransferTransactionChangedMessage(
                    GetTransferTransactionViewModel(), transaction.TransferTransaction));

                // Check if changing the payee turned this transaction into a transfer.
                if (transaction.TransferTransaction is not null)
                {
                    // Transfer transactions are uncategorized, so the view model should be updated to reflect this.
                    _chosenCategorySuggestion = new CategorySuggestion(null);
                    OnPropertyChanged(nameof(ChosenCategorySuggestion));

                    // Uncategorizing an existing categorized transaction may affect activity, so we need
                    // to update the total unbudgeted amounts in the budget.
                    _budgetService.UpdateTotalUnbudgetedAmounts(Date.Month, Date.Year);
                }

                if (TransferTransactionId != transaction.TransferTransaction?.Id)
                {
                    TransferTransactionId = transaction.TransferTransaction?.Id;
                    OnPropertyChanged(nameof(IsTransfer));
                }

                Messenger.Send(new AccountBalanceChangedMessage());
            }
        }

        partial void OnChosenCategorySuggestionChanged(CategorySuggestion oldValue, CategorySuggestion newValue)
        {
            if (oldValue.Id != newValue.Id)
            {
                _accountService.SetCategoryForTransaction(Id, newValue.Id);
                _budgetService.UpdateTotalUnbudgetedAmounts(Date.Month, Date.Year);
            }
        }

        public TransactionViewModel GetTransferTransactionViewModel()
        {
            if (TransferTransactionId is int transferTransactionId)
            {
                return Messenger.Send(new TransactionViewModelRequestMessage(), transferTransactionId);
            }

            return null;
        }
    }
}
