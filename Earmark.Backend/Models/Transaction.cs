using System;
using System.ComponentModel.DataAnnotations;

namespace Earmark.Backend.Models
{
    public class Transaction
    {
        /// <summary>
        /// The unique ID that identifies the transaction.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The date of the transaction.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The memo of the transaction.
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// The amount of the transaction.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// The account that the transaction is under.
        /// </summary>
        [Required]
        public Account Account { get; set; }

        /// <summary>
        /// The payee of the transaction.
        /// </summary>
        public Payee Payee { get; set; }

        /// <summary>
        /// The category of the transaction.
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// The related transaction associated with the transaction in a transfer between accounts.
        /// </summary>
        public Transaction TransferTransaction { get; set; }
    }
}
