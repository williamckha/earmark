using System;
using System.Collections.Generic;

namespace Earmark.Backend.Models
{
    public class CategoryGroup
    {
        /// <summary>
        /// The unique ID that identifies the category group.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The position of the category group in the user-defined ordering of groups.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// The name of the category group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Specifies whether the category group is an income category group.
        /// </summary>
        public bool IsIncome { get; set; }

        /// <summary>
        /// The categories in the category group.
        /// </summary>
        public List<Category> Categories { get; set; }
    }
}
