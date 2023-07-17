using System;
using System.Collections.Generic;

namespace Earmark.Backend.Models
{
    public class Account
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

        /// <summary>
        /// The transactions under the account.
        /// </summary>
        public List<Transaction> Transactions { get; set; }

        /// <summary>
        /// The payee that represents the account in transfers.
        /// </summary>
        public Payee TransferPayee { get; set; }
    }
}
