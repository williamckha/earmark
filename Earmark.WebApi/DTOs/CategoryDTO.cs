namespace Earmark.WebApi.DTOs
{
    public class CategoryDTO
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
    }
}
