using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Earmark.Backend.Models
{
    public class Category
    {
        /// <summary>
        /// The unique ID that identifies the category.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The position of the category when ordered within its group.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// The name of the category.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Specifies whether the category is an income category.
        /// </summary>
        public bool IsIncome { get; set; }

        /// <summary>
        /// The category group that contains the category.
        /// </summary>
        [Required]
        public CategoryGroup Group { get; set; }

        /// <summary>
        /// The budgeted amounts associated with the category.
        /// </summary>
        public List<BudgetedAmount> BudgetedAmounts { get; set; }

        /// <summary>
        /// The balance amounts associated with the category.
        /// </summary>
        public List<BalanceAmount> BalanceAmounts { get; set; }

        /// <summary>
        /// The transactions under the category.
        /// </summary>
        public List<Transaction> Transactions { get; set; }
    }
}
