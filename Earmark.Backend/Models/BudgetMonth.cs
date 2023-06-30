using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Earmark.Backend.Models
{
    public class BudgetMonth
    {
        public Guid Id { get; set; }
        
        public int Month { get; set; }

        public int Year { get; set; }

        public decimal TotalUnbudgeted { get; set; }

        [Required]
        public Budget Budget { get; set; }

        public List<BudgetedAmount> BudgetedAmounts { get; set; }

        public List<BalanceAmount> BalanceAmounts { get; set; }

        public List<RolloverAmount> RolloverAmounts { get; set; }
    }
}
