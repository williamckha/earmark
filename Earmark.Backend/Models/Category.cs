using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Earmark.Backend.Models
{
    public class Category
    {
        public Guid Id { get; set; }

        public int Position { get; set; }

        public string Name { get; set; }

        public bool IsIncome { get; set; }

        [Required]
        public CategoryGroup Group { get; set; }

        public List<BudgetedAmount> BudgetedAmounts { get; set; }

        public List<BalanceAmount> BalanceAmounts { get; set; }

        public List<RolloverAmount> RolloverAmounts { get; set; }

        public List<Transaction> Transactions { get; set; }
    }
}
