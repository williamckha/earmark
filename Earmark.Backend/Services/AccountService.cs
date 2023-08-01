using Earmark.Backend.Database;
using Earmark.Backend.Models;
using EntityFramework.DbContextScope.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;

namespace Earmark.Backend.Services
{
    public class AccountService : IAccountService
    {
        private IDbContextScopeFactory _dbContextScopeFactory;
        private IBudgetService _budgetService;

        public AccountService(
            IDbContextScopeFactory dbContextScopeFactory,
            IBudgetService budgetService)
        {
            _dbContextScopeFactory = dbContextScopeFactory;
            _budgetService = budgetService;
        }

        public IEnumerable<Account> GetAccounts()
        {
            using (var dbContextScope = _dbContextScopeFactory.CreateReadOnly())
            {
                return dbContextScope.DbContexts.Get<AppDbContext>().Accounts
                    .Include(x => x.TransferPayee)
                    .AsNoTracking()
                    .ToList();
            }
        }

        public IEnumerable<Transaction> GetTransactions()
        {
            using (var dbContextScope = _dbContextScopeFactory.CreateReadOnly())
            {
                return dbContextScope.DbContexts.Get<AppDbContext>().Transactions
                    .Include(x => x.Account)
                    .Include(x => x.Payee)
                    .Include(x => x.Category)
                    .Include(x => x.TransferTransaction)
                    .AsNoTracking()
                    .AsSplitQuery()
                    .ToList();
            }
        }

        public IEnumerable<Transaction> GetTransactions(IEnumerable<int> accountIds)
        {
            using (var dbContextScope = _dbContextScopeFactory.CreateReadOnly())
            {
                return dbContextScope.DbContexts.Get<AppDbContext>().Transactions
                    .Where(x => accountIds.Contains(x.Account.Id))
                    .Include(x => x.Account)
                    .Include(x => x.Payee)
                    .Include(x => x.Category)
                    .Include(x => x.TransferTransaction)
                    .AsNoTracking()
                    .AsSplitQuery()
                    .ToList();
            }
        }

        public int GetTotalBalanceForAccounts(IEnumerable<int> accountIds)
        {
            using (var dbContextScope = _dbContextScopeFactory.CreateReadOnly())
            {
                return dbContextScope.DbContexts.Get<AppDbContext>().Accounts
                    .Where(x => accountIds.Contains(x.Id))
                    .AsNoTracking()
                    .Sum(x => x.TotalBalance);
            }
        }

        public Account AddAccount(string name)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                if (dbContext.Accounts.Any(x => x.Name == name))
                    throw new ArgumentException("Account with the same name already exists.");

                var account = new Account()
                {
                    Name = name,
                    TotalBalance = 0
                };

                var transferPayee = new Payee()
                {
                    Name = name
                };
                account.TransferPayee = transferPayee;

                dbContext.Accounts.Add(account);
                dbContextScope.SaveChanges();
                return account;
            }
        }

        public Transaction AddTransaction(int accountId, DateTime date, int amount)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                var account = dbContext.Accounts.Find(accountId);

                if (account is null)
                    throw new ArgumentException("No account with the specified ID was found.");

                var transaction = new Transaction()
                {
                    Date = date,
                    Memo = string.Empty,
                    Amount = amount,
                    Account = account
                };

                account.TotalBalance += amount;

                dbContext.Transactions.Add(transaction);
                dbContextScope.SaveChanges();
                return transaction;
            }
        }

        public Transaction SetDateForTransaction(int transactionId, DateTime date)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                var transaction = dbContext.Transactions.Find(transactionId);

                if (transaction is null)
                    throw new ArgumentException("No transaction with the specified ID was found.");

                dbContext.Entry(transaction).Reference(x => x.Category).Load();
                dbContext.Entry(transaction).Reference(x => x.TransferTransaction).Load();

                var earlierDate = (transaction.Date < date) ? transaction.Date : date;

                if (transaction.Category is not null)
                {
                    _budgetService.UpdateBalanceAmounts(
                        transaction.Date.Month, transaction.Date.Year, transaction.Category.Id, -transaction.Amount);
                    _budgetService.UpdateBalanceAmounts(
                        date.Month, date.Year, transaction.Category.Id, transaction.Amount);
                }

                transaction.Date = date;
                if (transaction.TransferTransaction is not null)
                {
                    transaction.TransferTransaction.Date = transaction.Date;
                }

                dbContextScope.SaveChanges();
                return transaction;
            }
        }

        public Transaction SetMemoForTransaction(int transactionId, string memo)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                var transaction = dbContext.Transactions.Find(transactionId);

                if (transaction is null)
                    throw new ArgumentException("No transaction with the specified ID was found.");

                dbContext.Entry(transaction).Reference(x => x.TransferTransaction).Load();

                transaction.Memo = memo;
                if (transaction.TransferTransaction is not null)
                {
                    transaction.TransferTransaction.Memo = transaction.Memo;
                }

                dbContextScope.SaveChanges();
                return transaction;
            }
        }

        public Transaction SetAmountForTransaction(int transactionId, int amount)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                var transaction = dbContext.Transactions.Find(transactionId);

                if (transaction is null)
                    throw new ArgumentException("No transaction with the specified ID was found.");

                dbContext.Entry(transaction).Reference(x => x.Account).Load();
                dbContext.Entry(transaction).Reference(x => x.Category).Load();
                dbContext.Entry(transaction).Reference(x => x.TransferTransaction).Load();

                if (transaction.Category is not null)
                {
                    int amountChange = amount - transaction.Amount;
                    _budgetService.UpdateBalanceAmounts(
                        transaction.Date.Month, transaction.Date.Year, transaction.Category.Id, amountChange);
                }

                var changeInAmount = amount - transaction.Amount;
                transaction.Account.TotalBalance += changeInAmount;

                transaction.Amount = amount;
                if (transaction.TransferTransaction is not null)
                {
                    dbContext.Entry(transaction.TransferTransaction).Reference(x => x.Account).Load();
                    transaction.TransferTransaction.Account.TotalBalance -= changeInAmount;
                    transaction.TransferTransaction.Amount = -transaction.Amount;
                }

                dbContextScope.SaveChanges();
                return transaction;
            }
        }

        public Transaction SetAccountForTransaction(int transactionId, int accountId)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                var transaction = dbContext.Transactions.Find(transactionId);
                
                if (transaction is null)
                    throw new ArgumentException("No transaction with the specified ID was found.");

                dbContext.Entry(transaction).Reference(x => x.Account).Load();
                dbContext.Entry(transaction).Reference(x => x.TransferTransaction).Load();

                var account = dbContext.Accounts.Find(accountId);

                if (account is null)
                    throw new ArgumentException("No account with the specified ID was found.");

                transaction.Account.TotalBalance -= transaction.Amount;
                account.TotalBalance += transaction.Amount;

                transaction.Account = account;

                if (transaction.TransferTransaction is not null)
                {
                    dbContext.Entry(account).Reference(x => x.TransferPayee).Load();
                    transaction.TransferTransaction.Payee = account.TransferPayee;
                }

                dbContextScope.SaveChanges();
                return transaction;
            }
        }

        public Transaction SetPayeeForTransaction(int transactionId, int? payeeId)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                var transaction = dbContext.Transactions.Find(transactionId);

                if (transaction is null)
                    throw new ArgumentException("No transaction with the specified ID was found.");

                dbContext.Entry(transaction).Reference(x => x.TransferTransaction).Load();

                if (transaction.TransferTransaction is not null)
                {
                    RemoveTransaction(transaction.TransferTransaction.Id);
                }

                Payee payee = null;
                if (payeeId is not null)
                {
                    payee = dbContext.Payees.Find(payeeId);

                    if (payee is null)
                        throw new ArgumentException("No payee with the specified ID was found.");

                    dbContext.Entry(payee).Reference(x => x.TransferAccount).Load();
                }

                transaction.Payee = payee;

                if (payee?.TransferAccount is not null)
                {
                    var transferTransaction = AddTransaction(
                        payee.TransferAccount.Id, transaction.Date, -transaction.Amount);
                    transferTransaction.Memo = transaction.Memo;

                    dbContext.Entry(transaction).Reference(x => x.Account).Query().Include(x => x.TransferPayee).Load();
                    transferTransaction.Payee = transaction.Account.TransferPayee;

                    // Transfer transactions should be uncategorized. The newly created `transferTransaction` is
                    // already uncategorized, so we don't need to set its category to null.
                    SetCategoryForTransaction(transaction.Id, null);

                    transaction.TransferTransaction = transferTransaction;
                    transferTransaction.TransferTransaction = transaction;
                }

                dbContextScope.SaveChanges();
                return transaction;
            }
        }

        public Transaction SetCategoryForTransaction(int transactionId, int? categoryId)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                var transaction = dbContext.Transactions.Find(transactionId);

                if (transaction is null)
                    throw new ArgumentException("No transaction with the specified ID was found.");

                dbContext.Entry(transaction).Reference(x => x.Category).Load();

                if (transaction.Category is not null)
                {
                    _budgetService.UpdateBalanceAmounts(
                        transaction.Date.Month, transaction.Date.Year, transaction.Category.Id, -transaction.Amount);
                }

                Category category = null;
                if (categoryId is not null)
                {
                    category = dbContext.Categories.Find(categoryId);

                    if (category is null)
                        throw new ArgumentException("No category with the specified ID was found.");

                    _budgetService.UpdateBalanceAmounts(
                        transaction.Date.Month, transaction.Date.Year, category.Id, transaction.Amount);
                }

                transaction.Category = category;

                dbContextScope.SaveChanges();
                return transaction;
            }
        }

        public void RemoveAccount(int accountId)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                var account = dbContext.Accounts
                    .Include(x => x.Transactions)
                    .ThenInclude(x => x.TransferTransaction)
                    .Include(x => x.TransferPayee)
                    .FirstOrDefault(x => x.Id == accountId);

                if (account is null)
                    throw new ArgumentException("No account with the specified ID was found.");

                account.Transactions.ForEach(x => RemoveTransaction(x.Id));
                dbContext.Payees.Remove(account.TransferPayee);
                dbContext.Accounts.Remove(account);
                dbContextScope.SaveChanges();
            }
        }

        public void RemoveTransaction(int transactionId)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                var transaction = dbContext.Transactions.Find(transactionId);

                if (transaction is null)
                    throw new ArgumentException("No transaction with the specified ID was found.");

                dbContext.Entry(transaction).Reference(x => x.Account).Load();
                dbContext.Entry(transaction).Reference(x => x.Category).Load();
                dbContext.Entry(transaction).Reference(x => x.TransferTransaction).Load();

                transaction.Account.TotalBalance -= transaction.Amount;
                transaction.Account = null;

                if (transaction.TransferTransaction is not null)
                {
                    transaction.TransferTransaction.TransferTransaction = null;
                    transaction.TransferTransaction = null;
                }

                if (transaction.Category is not null)
                {
                    _budgetService.UpdateBalanceAmounts(
                        transaction.Date.Month, transaction.Date.Year, transaction.Category.Id, -transaction.Amount);
                }

                dbContextScope.SaveChanges();
            }
        }
    }
}
