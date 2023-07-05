using System;
using System.Collections.Generic;

namespace Earmark.Backend.Models
{
    public class Account
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int TotalBalance { get; set; }

        public List<Transaction> Transactions { get; set; }

        public Payee TransferPayee { get; set; }
    }
}
