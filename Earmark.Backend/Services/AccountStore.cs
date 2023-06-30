using Earmark.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Earmark.Backend.Services
{
    public class AccountStore : IAccountStore, IAccountDetailService
    {
        private List<Account> _accounts;

        public AccountStore()
        {
            _accounts = new List<Account>();
        }

        public IEnumerable<Account> GetAccounts()
        {
            return _accounts;
        }

        public decimal GetTotalActivityForMonth(int month, int year)
        {
            return _accounts
                .SelectMany(x => x.Transactions)
                .Where(x => x.Date.Year == year && x.Date.Month == month)
                .Where(x => x.Category?.IsIncome == false)
                .Sum(x => x.Amount);
        }

        public decimal GetTotalActivityForMonth(int month, int year, CategoryGroup categoryGroup)
        {
            return _accounts
                .SelectMany(x => x.Transactions)
                .Where(x => x.Date.Year == year && x.Date.Month == month)
                .Where(x => x.Category?.Group == categoryGroup)
                .Sum(x => x.Amount);
        }

        public decimal GetTotalActivityForMonth(int month, int year, Category category)
        {
            return _accounts
                .SelectMany(x => x.Transactions)
                .Where(x => x.Date.Year == year && x.Date.Month == month)
                .Where(x => x.Category == category)
                .Sum(x => x.Amount);
        }

        public decimal GetTotalIncomeForMonth(int month, int year)
        {
            return _accounts
                .SelectMany(x => x.Transactions)
                .Where(x => x.Date.Year == year && x.Date.Month == month)
                .Where(x => x.Category?.IsIncome == true)
                .Sum(x => x.Amount);
        }

        public void AddAccount(Account account)
        {
            if (account is null) throw new ArgumentNullException(nameof(account));

            _accounts.Add(account);
        }

        public void RemoveAccount(Account account)
        {
            if (account is null) throw new ArgumentNullException(nameof(account));

            _accounts.Remove(account);
        }
    }
}
