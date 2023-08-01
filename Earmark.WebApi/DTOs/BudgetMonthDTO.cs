namespace Earmark.WebApi.DTOs
{
    public class BudgetMonthDTO
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
    }
}
