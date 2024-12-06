using BankingControlPanel.Data.Models;
using System.Security.Claims;

namespace BankingControlPanel.API.Services.Jwt
{
    public interface IJwtAuthentication
    {
        Task<string> Authenticate(string email, string password, string role);  // Authenticate the user and return a token
        string GenerateToken(User user);                         // Generate a JWT token for the authenticated user
    }
}
