using BankingControlPanel.Data.Models;
using Microsoft.AspNetCore.Identity.Data;

namespace BankingControlPanel.API.Repositories.Users
{
    public interface IUser
    {
        Task<User> RegisterUser(User user);  // Register a new user
        Task<List<User>> GetAllUser(); //Get list of registered users
    }
}
