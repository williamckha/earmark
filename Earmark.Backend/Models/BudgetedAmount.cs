using System;
using System.ComponentModel.DataAnnotations;

namespace Earmark.Backend.Models
{
    public class BudgetedAmount
    {
        public Guid Id { get; set; }

        public int Amount { get; set; }

        [Required]
        public BudgetMonth Month { get; set; }

        [Required]
        public Category Category { get; set; }
    }
}
