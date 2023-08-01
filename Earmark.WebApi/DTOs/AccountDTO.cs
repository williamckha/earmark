namespace Earmark.WebApi.DTOs
{
    public class AccountDTO
    {
        /// <summary>
        /// The unique ID that identifies the account.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the account.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The total balance of the account.
        /// </summary>
        public int TotalBalance { get; set; }
    }
}
