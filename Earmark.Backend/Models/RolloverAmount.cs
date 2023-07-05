using System;
using System.ComponentModel.DataAnnotations;

namespace Earmark.Backend.Models
{
    public class RolloverAmount
    {
        /// <summary>
        /// The unique ID that identifies the rollover amount.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The amount in the category that was rolled over into the month.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// The month associated with the rollover amount.
        /// </summary>
        [Required]
        public BudgetMonth Month { get; set; }

        /// <summary>
        /// The category associated with the rollover amount.
        /// </summary>
        [Required]
        public Category Category { get; set; }
    }
}
