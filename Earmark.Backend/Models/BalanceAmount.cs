using System;
using System.ComponentModel.DataAnnotations;

namespace Earmark.Backend.Models
{
    public class BalanceAmount
    {
        /// <summary>
        /// The unique ID that identifies the balance amount.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The balance in the category for the month.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// The month associated with the balance amount.
        /// </summary>
        [Required]
        public BudgetMonth Month { get; set; }

        /// <summary>
        /// The category associated with the balance amount.
        /// </summary>
        [Required]
        public Category Category { get; set; }
    }
}
