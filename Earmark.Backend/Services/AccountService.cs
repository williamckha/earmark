using Earmark.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Earmark.Backend.Services
{
    public class AccountService : IAccountService
    {
        private IAccountStore _accountStore;
        private IBudgetService _budgetService;
        private IPayeeService _payeeService;

        public AccountService(
            IAccountStore accountStore,
            IBudgetService budgetService,
            IPayeeService payeeService)
        {
            _accountStore = accountStore;
            _budgetService = budgetService;
            _payeeService = payeeService;

            AddAccount("Chequing");
            AddAccount("Savings");
        }

        public IEnumerable<Account> GetAccounts()
        {
            return _accountStore.GetAccounts();
        }

        public Account AddAccount(string name)
        {
            if (_accountStore.GetAccounts().Any(x => x.Name == name))
                throw new ArgumentException("Account with the same name already exists.");

            var account = new Account()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Transactions = new List<Transaction>()
            };

            var transferPayee = _payeeService.AddPayee(name);
            transferPayee.TransferAccount = account;
            account.TransferPayee = transferPayee;

            _accountStore.AddAccount(account);

            return account;
        }

        public Transaction AddTransaction(Account account, DateTimeOffset date, decimal amount)
        {
            if (account is null) throw new ArgumentNullException(nameof(account));

            var transaction = new Transaction()
            {
                Id = Guid.NewGuid(),
                Date = date,
                Memo = string.Empty,
                Amount = amount,
                Account = account,
                Payee = null,
                Category = null,
                TransferTransaction = null
            };

            account.Transactions.Add(transaction);

            return transaction;
        }

        public void SetDateForTransaction(Transaction transaction, DateTimeOffset date)
        {
            if (transaction is null) throw new ArgumentNullException(nameof(transaction));

            var earlierDate = (transaction.Date < date) ? transaction.Date : date;

            if (transaction.Category is not null)
            {
                _budgetService.UpdateBalanceAmounts(
                    transaction.Date.Month, transaction.Date.Year, transaction.Category, -transaction.Amount);
                _budgetService.UpdateBalanceAmounts(
                    date.Month, date.Year, transaction.Category, transaction.Amount);
            }

            transaction.Date = date;

            if (transaction.TransferTransaction is not null)
            {
                transaction.TransferTransaction.Date = transaction.Date;
            }

            _budgetService.UpdateTotalUnbudgetedAmounts(earlierDate.Month, earlierDate.Year);
        }

        public void SetMemoForTransaction(Transaction transaction, string memo)
        {
            if (transaction is null) throw new ArgumentNullException(nameof(transaction));

            transaction.Memo = memo;

            if (transaction.TransferTransaction is not null)
            {
                transaction.TransferTransaction.Memo = transaction.Memo;
            }
        }

        public void SetAmountForTransaction(Transaction transaction, decimal amount)
        {
            if (transaction is null) throw new ArgumentNullException(nameof(transaction));

            if (transaction.Category is not null)
            {
                decimal amountChange = amount - transaction.Amount;
                _budgetService.UpdateBalanceAmounts(
                    transaction.Date.Month, transaction.Date.Year, transaction.Category, amountChange);
            }

            transaction.Amount = amount;

            if (transaction.TransferTransaction is not null)
            {
                transaction.TransferTransaction.Amount = -transaction.Amount;
            }

            _budgetService.UpdateTotalUnbudgetedAmounts(transaction.Date.Month, transaction.Date.Year);
        }

        public void SetAccountForTransaction(Transaction transaction, Account account)
        {
            if (transaction is null) throw new ArgumentNullException(nameof(transaction));
            if (account is null) throw new ArgumentNullException(nameof(account));
            if (account == transaction.Payee?.TransferAccount)
                throw new ArgumentException("Account and payee must be different for a transfer transaction.");

            if (transaction.Account == account) return;

            transaction.Account.Transactions.Remove(transaction);
            account.Transactions.Add(transaction);
            transaction.Account = account;

            if (transaction.TransferTransaction is not null)
            {
                transaction.TransferTransaction.Payee.Transactions.Remove(transaction.TransferTransaction);

                account.TransferPayee.Transactions.Add(transaction.TransferTransaction);
                transaction.TransferTransaction.Payee = account.TransferPayee;
            }
        }

        public void SetPayeeForTransaction(Transaction transaction, Payee payee)
        {
            if (transaction is null) throw new ArgumentNullException(nameof(transaction));
            if (payee == transaction.Account.TransferPayee)
                throw new ArgumentException("Account and payee must be different for a transfer transaction.");

            if (transaction.Payee == payee) return;

            if (transaction.TransferTransaction is not null)
            {
                transaction.TransferTransaction.TransferTransaction = null;
                RemoveTransaction(transaction.TransferTransaction);
                transaction.TransferTransaction = null;
            }

            transaction.Payee?.Transactions?.Remove(transaction);
            payee?.Transactions?.Add(transaction);
            transaction.Payee = payee;

            if (payee.TransferAccount is not null)
            {
                var transferTransaction = AddTransaction(
                    payee.TransferAccount, transaction.Date, -transaction.Amount);
                transferTransaction.Memo = transaction.Memo;

                transaction.Account.TransferPayee.Transactions.Add(transferTransaction);
                transferTransaction.Payee = transaction.Account.TransferPayee;

                SetCategoryForTransaction(transaction, null);
                SetCategoryForTransaction(transferTransaction, null);

                transaction.TransferTransaction = transferTransaction;
                transferTransaction.TransferTransaction = transaction;
            }
        }

        public void SetCategoryForTransaction(Transaction transaction, Category category)
        {
            if (transaction is null) throw new ArgumentNullException(nameof(transaction));
            if (transaction.TransferTransaction is not null && category is not null)
                throw new ArgumentException("Transfer transactions cannot be categorized.");

            if (transaction.Category == category) return;

            if (transaction.Category is not null)
            {
                _budgetService.UpdateBalanceAmounts(
                    transaction.Date.Month, transaction.Date.Year, transaction.Category, -transaction.Amount);
            }

            if (category is not null)
            {
                _budgetService.UpdateBalanceAmounts(
                    transaction.Date.Month, transaction.Date.Year, category, transaction.Amount);
            }

            transaction.Category?.Transactions?.Remove(transaction);
            category?.Transactions?.Add(transaction);
            transaction.Category = category;

            _budgetService.UpdateTotalUnbudgetedAmounts(transaction.Date.Month, transaction.Date.Year);
        }

        public void RemoveAccount(Account account)
        {
            if (account is null) throw new ArgumentNullException(nameof(account));

            while (account.Transactions.Any())
            {
                RemoveTransaction(account.Transactions.First());
            }

            account.TransferPayee.TransferAccount = null;
            account.TransferPayee = null;

            _accountStore.RemoveAccount(account);
        }

        public void RemoveTransaction(Transaction transaction)
        {
            if (transaction is null) throw new ArgumentNullException(nameof(transaction));

            void SeverRelationshipsForTransaction(Transaction transaction)
            {
                if (transaction.Category is not null)
                {
                    _budgetService.UpdateBalanceAmounts(
                        transaction.Date.Month, transaction.Date.Year, transaction.Category, -transaction.Amount);
                }

                transaction.Account.Transactions.Remove(transaction);
                transaction.Account = null;

                transaction.Payee?.Transactions?.Remove(transaction);
                transaction.Payee = null;

                transaction.Category?.Transactions?.Remove(transaction);
                transaction.Category = null;

                _budgetService.UpdateTotalUnbudgetedAmounts(transaction.Date.Month, transaction.Date.Year);
            }

            SeverRelationshipsForTransaction(transaction);
            if (transaction.TransferTransaction is not null)
            {
                SeverRelationshipsForTransaction(transaction.TransferTransaction);
                transaction.TransferTransaction.TransferTransaction = null;
                transaction.TransferTransaction = null;
            }
        }
    }
}
