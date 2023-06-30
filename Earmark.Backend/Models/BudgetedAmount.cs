using System;
using System.ComponentModel.DataAnnotations;

namespace Earmark.Backend.Models
{
    public class BudgetedAmount
    {
        public Guid Id { get; set; }

        public decimal Amount { get; set; }

        [Required]
        public BudgetMonth Month { get; set; }

        [Required]
        public Category Category { get; set; }
    }
}
