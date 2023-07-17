using System;
using System.Collections.Generic;

namespace Earmark.Backend.Models
{
    public class Payee
    {
        /// <summary>
        /// The unique ID that identifies the payee.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the payee.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The transactions under the payee.
        /// </summary>
        public List<Transaction> Transactions { get; set; }

        /// <summary>
        /// The associated account that the payee transfers to/from, if the payee
        /// is a transfer payee.
        /// </summary>
        public Account TransferAccount { get; set; }
    }
}
