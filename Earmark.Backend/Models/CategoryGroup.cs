using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Earmark.Backend.Models
{
    public class CategoryGroup
    {
        public Guid Id { get; set; }

        public int Position { get; set; }

        public string Name { get; set; }

        public bool IsIncome { get; set; }

        [Required]
        public Budget Budget { get; set; }

        public List<Category> Categories { get; set; }
    }
}
