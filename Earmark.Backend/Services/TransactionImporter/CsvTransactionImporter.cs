using CsvHelper;
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
                var csvTransactions = csv.GetRecords<CsvTransaction>();
                foreach (var csvTransaction in csvTransactions)
                {
                    var account =
                        _accountService.GetAccounts().FirstOrDefault(x => x.Name == csvTransaction.Account) ??
                        _accountService.AddAccount(csvTransaction.Account);

                    var transaction = _accountService.AddTransaction(
                        account, csvTransaction.Date, csvTransaction.Amount);

                    if (!string.IsNullOrEmpty(csvTransaction.Memo))
                    {
                        _accountService.SetMemoForTransaction(transaction, csvTransaction.Memo);
                    }

                    var payee = 
                        _payeeService.GetPayees().FirstOrDefault(x => x.Name == csvTransaction.Payee) ??
                        _payeeService.AddPayee(csvTransaction.Payee);
                    _accountService.SetPayeeForTransaction(transaction, payee);

                    transactions.Add(transaction);
                }
            }

            return transactions;
        }
    }

    public class CsvTransaction
    {
        public DateTimeOffset Date { get; set; }

        public string Account { get; set; }

        public string Payee { get; set; }

        public string Memo { get; set; }

        public decimal Amount { get; set; }
    }
}
