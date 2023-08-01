namespace Earmark.WebApi.DTOs
{
    public class TransactionDTO
    {
        /// <summary>
        /// The unique ID that identifies the transaction.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The date of the transaction.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The memo of the transaction.
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// The amount of the transaction.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// The name of the account that the transaction is under.
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// The name of the payee that the transaction is under.
        /// </summary>
        public string PayeeName { get; set; }

        /// <summary>
        /// The name of the category that the transaction is under.
        /// </summary>
        public string CategoryName { get; set; }
    }
}
