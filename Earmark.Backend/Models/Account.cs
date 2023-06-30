using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Earmark.Backend.Models
{
    public class Account
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<Transaction> Transactions { get; set; }

        [Required]
        public Payee TransferPayee { get; set; }
    }
}
