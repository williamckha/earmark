using Earmark.Backend.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Earmark.Backend.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Budget> Budgets { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Payee> Payees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "C:\\Users\\willi\\Desktop\\Budget.earmark";
            optionsBuilder.UseSqlite(connectionStringBuilder.ToString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payee>()
                .HasOne(e => e.TransferAccount)
                .WithOne(e => e.TransferPayee)
                .HasForeignKey<Account>();
        }
    }
}
