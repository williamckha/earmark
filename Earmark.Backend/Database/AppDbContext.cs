using Earmark.Backend.Models;
using EntityFramework.DbContextScope.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Earmark.Backend.Database
{
    public class AppDbContext : DbContext, IDbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Payee> Payees { get; set; }

        public DbSet<BudgetMonth> BudgetMonths { get; set; }
        
        public DbSet<CategoryGroup> CategoryGroups { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<BudgetedAmount> BudgetedAmounts { get; set; }

        public DbSet<BalanceAmount> BalanceAmounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "Budget.earmark";
            optionsBuilder.UseSqlite(connectionStringBuilder.ToString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payee>()
                .HasOne(e => e.TransferAccount)
                .WithOne(e => e.TransferPayee)
                .HasForeignKey<Account>();

            modelBuilder.Entity<Transaction>()
                .HasOne(e => e.TransferTransaction)
                .WithOne()
                .HasForeignKey<Transaction>()
                .IsRequired(false);
        }
    }
}
