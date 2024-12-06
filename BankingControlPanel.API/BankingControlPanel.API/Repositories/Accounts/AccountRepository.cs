using BankingControlPanel.Data.Data;
using BankingControlPanel.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankingControlPanel.API.Repositories.Accounts
{
    public class AccountRepository : IAccount
    {
        private readonly BankingControlPanelDbContext _context;
        public AccountRepository(BankingControlPanelDbContext context)
        {
            _context = context;
        }
        public async Task<List<Account>> GetAllAccount()
        {
            var accounts = await _context.Accounts.ToListAsync();
            return accounts;
        }
        public async Task<Account> GetAccountById(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            return account;
        }
        public async Task<Account> AddAccount(Account account)
        {
           // var client = await _context.Clients.FirstOrDefaultAsync(c => c.Email == account.Email);

            //if (client == null)
            //{
            //    return null;
            //}
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<Account> UpdateAccount(int id, Account account)
        {
            var existingAccount = await _context.Accounts.FindAsync(id);
            if (id != account.AccountId)
            {
                return null;
            }

            //// Update the fields of the existing account (excluding the Client relationship)
            //existingAccount.Email = account.Email;
            //existingAccount.Password = account.Password;
            //existingAccount.Role = account.Role;


            await _context.SaveChangesAsync();

            return existingAccount;
            //if (id != account.AccountId)
            //{
            //    return null;
            //}
            //Context.Entry(account).State = EntityState.Modified;
            //await Context.SaveChangesAsync();
            //return account;
        }

        public async Task<Account> DeleteAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
            return account;
        }
    }
}
