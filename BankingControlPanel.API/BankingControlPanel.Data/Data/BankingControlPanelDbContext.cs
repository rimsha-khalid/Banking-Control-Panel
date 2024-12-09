using BankingControlPanel.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Data.Data
{
    // DbContext for the Banking Control Panel application
    public class BankingControlPanelDbContext : DbContext
    {
        // Constructor that accepts DbContextOptions and passes them to the base class
        public BankingControlPanelDbContext(DbContextOptions<BankingControlPanelDbContext> options) : base(options) { }
        public DbSet<Client> Clients { get; set; } // DbSet representing the Clients table in the database
        public DbSet<Address> Addresses { get; set; } // DbSet representing the Addresses table in the database
        public DbSet<Account> Accounts { get; set; } // DbSet representing the Accounts table in the database
        public DbSet<Search> Searches { get; set; } // DbSet representing the Searches table in the database
        public DbSet<User> Users { get; set; } // DbSet representing the Users table in the database
        public DbSet<Pagination> Paginations { get; set; } // DbSet representing the Pagination table in the databas


        // Configures the model and its relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Convert the Sex enum to a string in the database
            modelBuilder.Entity<Client>()
                .Property(c => c.Sex)
                .HasConversion(
                    v => v.ToString(),  // Convert Sex enum to string when saving to DB
                    v => (Sex)Enum.Parse(typeof(Sex), v)  // Convert string back to Sex enum when reading from DB
                );
            // Convert the AccountType enum to a string in the database
            modelBuilder.Entity<Account>()
                .Property(a => a.AccountType)
                .HasConversion(
                    v => v.ToString(),  // Convert AccountType enum to string when saving to DB
                    v => (AccountType)Enum.Parse(typeof(AccountType), v)  // Convert string back to AccountType enum when reading from DB
                );


        }
    }
}
