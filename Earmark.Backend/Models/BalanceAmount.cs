using System;
using System.ComponentModel.DataAnnotations;

namespace Earmark.Backend.Models
{
    public class BalanceAmount
    {
        /// <summary>
        /// The unique ID that identifies the balance amount.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The balance for the month in the category.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// The part of the balance amount for the month in the category that was
        /// rolled over from previous months.
        /// </summary>
        public int RolloverAmount { get; set; }

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
