using System;
using System.ComponentModel.DataAnnotations;

namespace Earmark.Backend.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }

        public DateTimeOffset Date { get; set; }

        public string Memo { get; set; }

        public decimal Amount { get; set; }

        [Required]
        public Account Account { get; set; }

        public Payee Payee { get; set; }

        public Category Category { get; set; }

        public Transaction TransferTransaction { get; set; }
    }
}
