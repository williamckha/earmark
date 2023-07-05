﻿using CsvHelper;
using Earmark.Backend.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Earmark.Backend.Services.TransactionImporter
{
    public class CsvTransactionImporter : ITransactionImporter
    {
        private IAccountService _accountService;
        private IPayeeService _payeeService;

        public CsvTransactionImporter(
            IAccountService accountService,
            IPayeeService payeeService)
        {
            _accountService = accountService;
            _payeeService = payeeService;
        }

        public IEnumerable<Transaction> ImportTransactionsFromFile(string filePath)
        {
            var transactions = new List<Transaction>();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var accounts = new List<Account>(_accountService.GetAccounts());
                var payees = new List<Payee>(_payeeService.GetPayees());

                var csvTransactions = csv.GetRecords<CsvTransaction>();
                foreach (var csvTransaction in csvTransactions)
                {
                    var account = accounts.FirstOrDefault(x => x.Name == csvTransaction.Account);
                    if (account is null)
                    {
                        account = _accountService.AddAccount(csvTransaction.Account);
                        accounts.Add(account);
                    }

                    var transaction = _accountService.AddTransaction(
                        account.Id, csvTransaction.Date, csvTransaction.Amount);

                    if (!string.IsNullOrEmpty(csvTransaction.Memo))
                    {
                        _accountService.SetMemoForTransaction(transaction.Id, csvTransaction.Memo);
                        transaction.Memo = csvTransaction.Memo;
                    }

                    var payee = payees.FirstOrDefault(x => x.Name == csvTransaction.Payee);
                    if (payee is null)
                    {
                        payee = _payeeService.AddPayee(csvTransaction.Payee);
                        payees.Add(payee);
                    }
                        
                    _accountService.SetPayeeForTransaction(transaction.Id, payee.Id);
                    transaction.Payee = payee;

                    transactions.Add(transaction);
                }
            }

            return transactions;
        }
    }

    public class CsvTransaction
    {
        public DateTime Date { get; set; }

        public string Account { get; set; }

        public string Payee { get; set; }

        public string Memo { get; set; }

        public int Amount { get; set; }
    }
}
