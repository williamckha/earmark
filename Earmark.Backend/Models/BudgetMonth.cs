using System;
using System.Collections.Generic;

namespace Earmark.Backend.Models
{
    public class BudgetMonth
    {
        /// <summary>
        /// The unique ID that identifies the budget month.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The month of the budget month.
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// The year of the budget month.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// The total amount unbudgeted for the month.
        /// </summary>
        public int TotalUnbudgeted { get; set; }

        /// <summary>
        /// The budgeted amounts for the month.
        /// </summary>
        public List<BudgetedAmount> BudgetedAmounts { get; set; }

        /// <summary>
        /// The balance amounts for the month.
        /// </summary>
        public List<BalanceAmount> BalanceAmounts { get; set; }
    }
}
