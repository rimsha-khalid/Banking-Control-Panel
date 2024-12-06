using BankingControlPanel.Data.Models;

namespace BankingControlPanel.API.Repositories.Accounts
{
    public interface IAccount
    {
        Task<List<Account>> GetAllAccount();
        Task<Account> GetAccountById(int id);
        Task<Account> AddAccount(Account account);
        Task<Account> UpdateAccount(int id, Account account);
        Task<Account> DeleteAccount(int id);
    }
}
