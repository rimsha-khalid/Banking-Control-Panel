using BankingControlPanel.Data.Data;
using BankingControlPanel.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingControlPanel.API.Repositories.Users
{
    public class UserRepository : IUser
    {
        // Dependency injection of the BankingControlPanelDbContext 
        private readonly BankingControlPanelDbContext _context;
        // Constructor that initializes the BankingControlPanelDbContext instance
        public UserRepository(BankingControlPanelDbContext context)
        {
                _context = context;
        }
        public async Task<User> RegisterUser(User user)
        {
            // Check if a user with the same email already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == user.Email);

            if (existingUser != null)
            {
                // Throw an exception if the email address is stored in database
                throw new InvalidOperationException("A user with this email address already exists.");
            }
       
            // Save new user to database
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();  
            return user;
        }
    }
}
