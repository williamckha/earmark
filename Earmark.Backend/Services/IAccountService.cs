using Earmark.Backend.Models;
using System;
using System.Collections.Generic;

namespace Earmark.Backend.Services
{
    public interface IAccountService
    {
        /// <summary>
        /// Gets all accounts in the database.
        /// </summary>
        /// <returns>All the accounts in the database.</returns>
        IEnumerable<Account> GetAccounts();

        /// <summary>
        /// Gets all transactions in the database.
        /// </summary>
        /// <returns>All the transactions in the database.</returns>
        IEnumerable<Transaction> GetTransactions();

        /// <summary>
        /// Gets all transactions in the database that are under the specified accounts.
        /// </summary>
        /// <param name="accountIds">The IDs of the accounts to find transactions under.</param>
        /// <returns>All transactions in the database that are under the specified accounts.</returns>
        IEnumerable<Transaction> GetTransactions(IEnumerable<int> accountIds);

        /// <summary>
        /// Gets the sum of the total balances of the specified accounts.
        /// </summary>
        /// <param name="accountIds">The IDs of the accounts.</param>
        /// <returns>The sum of the total balances of the specified accounts.</returns>
        int GetTotalBalanceForAccounts(IEnumerable<int> accountIds);

        /// <summary>
        /// Adds an account with the specified name.
        /// </summary>
        /// <param name="name">The name of the account.</param>
        /// <returns>The added account.</returns>
        Account AddAccount(string name);

        /// <summary>
        /// Adds a transaction under the specified account.
        /// </summary>
        /// <param name="accountId">The ID of the account to assign the transaction under.</param>
        /// <param name="date">The date of the transaction.</param>
        /// <param name="amount">The amount of the transaction.</param>
        /// <returns>The added transaction.</returns>
        Transaction AddTransaction(int accountId, DateTime date, int amount);

        /// <summary>
        /// Sets the date for the transaction.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction.</param>
        /// <param name="date">The date of the transaction.</param>
        /// <return>The updated transaction with the changed properties set to their new values.</return>
        Transaction SetDateForTransaction(int transactionId, DateTime date);

        /// <summary>
        /// Sets the memo for the transaction.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction.</param>
        /// <param name="memo">The memo of the transaction.</param>
        /// <return>The updated transaction with the changed properties set to their new values.</return>
        Transaction SetMemoForTransaction(int transactionId, string memo);

        /// <summary>
        /// Sets the amount for the transaction.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction.</param>
        /// <param name="amount">The amount of the transaction.</param>
        /// <return>The updated transaction with the changed properties set to their new values.</return>
        Transaction SetAmountForTransaction(int transactionId, int amount);

        /// <summary>
        /// Sets the account for the transaction.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction.</param>
        /// <param name="accountId">The ID of the account to assign the transaction under.</param>
        /// <return>The updated transaction with the changed properties set to their new values.</return>
        Transaction SetAccountForTransaction(int transactionId, int accountId);

        /// <summary>
        /// Sets the payee for the transaction.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction.</param>
        /// <param name="payeeId">
        /// The ID of the payee to assign the transaction under. 
        /// Setting payee to null will leave the payee for the transaction blank.
        /// </param>
        /// <return>The updated transaction with the changed properties set to their new values.</return>
        Transaction SetPayeeForTransaction(int transactionId, int? payeeId);

        /// <summary>
        /// Sets the category for the transaction.
        /// </summary>
        /// <param name="transactionId">The ID of the transaction.</param>
        /// <param name="categoryId">
        /// The ID of the category to assign the transaction under. 
        /// Setting category to null will leave the category for the transaction blank.
        /// </param>
        /// <return>The updated transaction with the changed properties set to their new values.</return>
        Transaction SetCategoryForTransaction(int transactionId, int? categoryId);

        /// <summary>
        /// Removes the account and all of the transactions under the account.
        /// </summary>
        /// <param name="accountId">The ID of the account to remove.</param>
        void RemoveAccount(int accountId);

        /// <summary>
        /// Removes the transaction.
        /// </summary>
        /// <remarks>
        /// If the transaction to remove is part of a transfer, its related transfer transaction will remain and
        /// stay existing as a normal transaction. Both transfer transactions must be removed separately in order
        /// to fully remove the transfer.
        /// </remarks>
        /// <param name="transactionId">The ID of the transaction to remove.</param>
        void RemoveTransaction(int transactionId);
    }
}
