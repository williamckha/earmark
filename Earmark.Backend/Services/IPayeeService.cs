using Earmark.Backend.Models;
using System.Collections.Generic;

namespace Earmark.Backend.Services
{
    public interface IPayeeService
    {
        /// <summary>
        /// Gets all payees.
        /// </summary>
        /// <returns>All the payees.</returns>
        IEnumerable<Payee> GetPayees();

        /// <summary>
        /// Adds a payee with the specified name.
        /// </summary>
        /// <param name="name">The name of the payee.</param>
        /// <returns>The added payee.</returns>
        Payee AddPayee(string name);

        /// <summary>
        /// Removes the payee.
        /// </summary>
        /// <param name="payee">The payee to remove.</param>
        void RemovePayee(Payee payee);
    }
}
