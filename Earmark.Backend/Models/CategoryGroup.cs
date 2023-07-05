using System;
using System.Collections.Generic;

namespace Earmark.Backend.Models
{
    public class CategoryGroup
    {
        public Guid Id { get; set; }

        public int Position { get; set; }

        public string Name { get; set; }

        public bool IsIncome { get; set; }

        public List<Category> Categories { get; set; }
    }
}
