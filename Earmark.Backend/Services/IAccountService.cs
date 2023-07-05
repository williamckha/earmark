using Earmark.Backend.Models;
using System;
using System.Collections.Generic;

namespace Earmark.Backend.Services
{
    public interface IAccountService
    {
        IEnumerable<Account> GetAccounts();

        IEnumerable<Transaction> GetTransactions(IEnumerable<Guid> accountIds);

        int GetTotalBalanceForAccounts(IEnumerable<Guid> accountIds);

        Account AddAccount(string name);

        Transaction AddTransaction(Guid accountId, DateTime date, int amount);

        Transaction SetDateForTransaction(Guid transactionId, DateTime date);

        Transaction SetMemoForTransaction(Guid transactionId, string memo);

        Transaction SetAmountForTransaction(Guid transactionId, int amount);

        /// <summary>
        /// Sets the account for the transaction.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction.</param>
        /// <param name="accountId">The ID of the account to assign the transaction under.</param>
        /// <return>The updated transaction with the changed properties set to their new values.</return>
        Transaction SetAccountForTransaction(Guid transactionId, Guid accountId);

        /// <summary>
        /// Sets the payee for the transaction.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction.</param>
        /// <param name="payeeId">
        /// The ID of the payee to assign the transaction under. 
        /// Setting payee to null will leave the payee for the transaction blank.
        /// </param>
        /// <return>The updated transaction with the changed properties set to their new values.</return>
        Transaction SetPayeeForTransaction(Guid transactionId, Guid? payeeId);

        /// <summary>
        /// Sets the category for the transaction.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction.</param>
        /// <param name="categoryId">
        /// The ID of the category to assign the transaction under. 
        /// Setting category to null will leave the category for the transaction blank.
        /// </param>
        /// <return>The updated transaction with the changed properties set to their new values.</return>

        Transaction SetCategoryForTransaction(Guid transactionId, Guid? categoryId);

        /// <summary>
        /// Removes the account and all of the transactions under the account.
        /// </summary>
        /// <param name="accountId">The ID of the account to remove.</param>
        void RemoveAccount(Guid accountId);

        /// <summary>
        /// Removes the transaction.
        /// </summary>
        /// <remarks>
        /// If the transaction to remove is part of a transfer, its related transfer transaction will remain and
        /// stay existing as a normal transaction. Both transfer transactions must be removed separately in order
        /// to fully remove the transfer.
        /// </remarks>
        /// <param name="transactionId">The ID of the transaction to remove.</param>
        void RemoveTransaction(Guid transactionId);
    }
}
