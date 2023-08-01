using Earmark.Backend.Database;
using Earmark.Backend.Models;
using EntityFramework.DbContextScope.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Earmark.Backend.Services
{
    public class PayeeService : IPayeeService
    {
        private IDbContextScopeFactory _dbContextScopeFactory;

        public PayeeService(IDbContextScopeFactory dbContextScopeFactory)
        {
            _dbContextScopeFactory = dbContextScopeFactory;
        }

        public IEnumerable<Payee> GetPayees()
        {
            using (var dbContextScope = _dbContextScopeFactory.CreateReadOnly())
            {
                return dbContextScope.DbContexts.Get<AppDbContext>().Payees
                    .Include(x => x.TransferAccount)
                    .AsNoTracking()
                    .ToList();
            }
        }

        public Payee AddPayee(string name)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                if (dbContext.Payees.Any(x => x.Name == name))
                    throw new ArgumentException("Payee with the same name already exists.");

                var payee = new Payee()
                {
                    Name = name
                };

                dbContext.Payees.Add(payee);
                dbContextScope.SaveChanges();
                return payee;
            }
        }

        public void RemovePayee(int payeeId)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<AppDbContext>();

                var payee = dbContext.Payees.Find(payeeId);

                if (payee is null)
                    throw new ArgumentException("No payee with the specified ID was found."); 

                dbContext.Entry(payee).Reference(x => x.TransferAccount).Load();

                if (payee.TransferAccount is not null)
                    throw new ArgumentException("Transfer payees cannot be removed.");

                dbContext.Payees.Remove(payee);
                dbContextScope.SaveChanges();
            }
        }
    }
}
