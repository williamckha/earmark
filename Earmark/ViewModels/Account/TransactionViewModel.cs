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
        private IPayeeService _payeeService;
        private ICategoriesService _categoriesService;

        private Transaction _transaction;

        public Guid Id => _transaction.Id;

        public bool IsTransfer => _transaction.TransferTransaction is not null;

        public TransactionViewModel TransferTransaction => 
            Messenger.Send(new TransactionViewModelRequestMessage(_transaction.TransferTransaction));

        public DateTimeOffset Date
        {
            get => _transaction.Date;
            set
            {
                if (_transaction.Date != value)
                {
                    _accountService.SetDateForTransaction(_transaction, value);
                    TransferTransaction?.OnPropertyChanged(nameof(Date));
                }
            }
        }

        public string Memo
        {
            get => _transaction.Memo;
            set
            {
                if (_transaction.Memo != value)
                {
                    _accountService.SetMemoForTransaction(_transaction, value);
                    TransferTransaction?.OnPropertyChanged(nameof(Memo));
                }
            }
        }

        public decimal Amount
        {
            get => _transaction.Amount;
            set
            {
                if (_transaction.Amount != value)
                {
                    _accountService.SetAmountForTransaction(_transaction, value);
                    TransferTransaction?.OnPropertyChanged(nameof(Amount));
                    Messenger.Send(new AccountBalanceChangedMessage());
                }
            }
        }

        public AccountSuggestion ChosenAccountSuggestion
        {
            get => new AccountSuggestion(_transaction.Account);
            set
            {
                if (_transaction.Account.Id != value.Id)
                {
                    var account = _accountService.GetAccounts().First(x => x.Id == value.Id);
                    _accountService.SetAccountForTransaction(_transaction, account);

                    TransferTransaction?.OnPropertyChanged(nameof(ChosenPayeeSuggestion));
                    Messenger.Send(new AccountBalanceChangedMessage());
                }
            }
        }

        public PayeeSuggestion ChosenPayeeSuggestion
        {
            get => new PayeeSuggestion(_transaction.Payee);
            set
            {
                if (_transaction.Payee?.Id != value.Id)
                {
                    var oldTransferTransaction = _transaction.TransferTransaction;

                    var payee = _payeeService.GetPayees().FirstOrDefault(x => x.Id == value.Id);
                    _accountService.SetPayeeForTransaction(_transaction, payee);

                    if (_transaction.TransferTransaction is not null)
                    {
                        OnPropertyChanged(nameof(ChosenCategorySuggestion));
                    }

                    if (_transaction.TransferTransaction?.Id != oldTransferTransaction?.Id)
                    {
                        OnPropertyChanged(nameof(IsTransfer));
                    }

                    Messenger.Send(new TransferTransactionChangedMessage(
                        oldTransferTransaction, _transaction.TransferTransaction));
                }
            }
        }

        public CategorySuggestion ChosenCategorySuggestion
        {
            get => new CategorySuggestion(_transaction.Category);
            set
            {
                if (_transaction.TransferTransaction is null && _transaction.Category?.Id != value.Id)
                {
                    var category = _categoriesService.GetCategories().FirstOrDefault(x => x.Id == value.Id);
                    _accountService.SetCategoryForTransaction(_transaction, category);
                }
            }
        }

        public TransactionViewModel(
            IAccountService accountService,
            IPayeeService payeeService,
            ICategoriesService categoriesService,
            Transaction transaction) : base(StrongReferenceMessenger.Default)
        {
            _accountService = accountService;
            _payeeService = payeeService;
            _categoriesService = categoriesService;

            _transaction = transaction;
        }
    }
}
