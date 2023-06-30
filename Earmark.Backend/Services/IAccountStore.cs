using Earmark.Backend.Models;
using System.Collections.Generic;

namespace Earmark.Backend.Services
{
    public interface IAccountStore
    {
        IEnumerable<Account> GetAccounts();

        void AddAccount(Account account);

        void RemoveAccount(Account account);
    }
}
