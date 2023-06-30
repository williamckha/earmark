using System;
using System.Collections.Generic;

namespace Earmark.Backend.Models
{
    public class Budget
    {
        public Guid Id { get; set; }

        public List<BudgetMonth> Months { get; set; }

        public List<CategoryGroup> CategoryGroups { get; set; }
    }
}
