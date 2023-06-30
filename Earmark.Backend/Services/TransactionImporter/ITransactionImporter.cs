using Earmark.Backend.Models;
using System.Collections.Generic;

namespace Earmark.Backend.Services.TransactionImporter
{
    public interface ITransactionImporter
    {
        /// <summary>
        /// Imports transactions from the given file.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        IEnumerable<Transaction> ImportTransactionsFromFile(string filePath);
    }
}
