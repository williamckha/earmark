using System;
using System.Collections.Generic;

namespace Earmark.Backend.Models
{
    public class Payee
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<Transaction> Transactions { get; set; }

        public Account TransferAccount { get; set; }
    }
}
