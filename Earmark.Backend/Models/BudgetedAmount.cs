using System;
using System.ComponentModel.DataAnnotations;

namespace Earmark.Backend.Models
{
    public class BudgetedAmount
    {
        /// <summary>
        /// The unique ID that identifies the budgeted amount.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The amount budgeted for the month in the category.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// The month associated with the budgeted amount.
        /// </summary>
        [Required]
        public BudgetMonth Month { get; set; }

        /// <summary>
        /// The category associated with the budgeted amount.
        /// </summary>
        [Required]
        public Category Category { get; set; }
    }
}
