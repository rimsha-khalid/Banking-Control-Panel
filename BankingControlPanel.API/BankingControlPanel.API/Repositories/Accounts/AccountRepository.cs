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
            else
            {
                existingAccount.AccountNumber = account.AccountNumber;
                existingAccount.Balance = account.Balance;
                existingAccount.AccountType = account.AccountType;

            }

            await _context.SaveChangesAsync();
            return existingAccount;
 
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
