using Earmark.Backend.Models;
using System;
using System.Collections.Generic;

namespace Earmark.Backend.Services
{
    public interface IAccountService
    {
        IEnumerable<Account> GetAccounts();

        Account AddAccount(string name);

        Transaction AddTransaction(Account account, DateTimeOffset date, decimal amount);

        void SetDateForTransaction(Transaction transaction, DateTimeOffset date);

        void SetMemoForTransaction(Transaction transaction, string memo);

        void SetAmountForTransaction(Transaction transaction, decimal amount);

        /// <summary>
        /// Sets the account for the transaction.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="account">The account to assign the transaction under.</param>
        void SetAccountForTransaction(Transaction transaction, Account account);

        /// <summary>
        /// Sets the payee for the transaction.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="payee">
        /// The payee to assign the transaction under. 
        /// Setting payee to null will leave the payee for the transaction blank.
        /// </param>
        void SetPayeeForTransaction(Transaction transaction, Payee payee);

        /// <summary>
        /// Sets the category for the transaction.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="category">
        /// The category to assign the transaction under. 
        /// Setting category to null will leave the category for the transaction blank.
        /// </param>
        void SetCategoryForTransaction(Transaction transaction, Category category);

        /// <summary>
        /// Removes the account and all of the transactions under the account.
        /// </summary>
        /// <param name="account">The account to remove.</param>
        void RemoveAccount(Account account);

        /// Removes the transaction.
        /// </summary>
        /// <param name="transaction">The transaction to remove.</param>
        void RemoveTransaction(Transaction transaction);
    }
}
