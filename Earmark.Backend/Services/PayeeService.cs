using Earmark.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Earmark.Backend.Services
{
    public class PayeeService : IPayeeService
    {
        private List<Payee> _payees;

        public PayeeService()
        {
            _payees = new List<Payee>();
        }

        public IEnumerable<Payee> GetPayees()
        {
            return _payees;
        }

        public Payee AddPayee(string name)
        {
            if (_payees.Any(x => x.Name == name))
                throw new ArgumentException("Payee with the same name already exists.");

            var payee = new Payee()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Transactions = new List<Transaction>(),
                TransferAccount = null
            };

            _payees.Add(payee);

            return payee;
        }

        public void RemovePayee(Payee payee)
        {
            if (payee is null) throw new ArgumentNullException(nameof(payee));
            if (payee.TransferAccount is not null)
                throw new ArgumentException("Transfer payees cannot be removed.");

            while (payee.Transactions.Any())
            {
                var transaction = payee.Transactions.First();
                payee.Transactions.Remove(transaction);
                transaction.Payee = null;
            }

            _payees.Remove(payee);
        }
    }
}
