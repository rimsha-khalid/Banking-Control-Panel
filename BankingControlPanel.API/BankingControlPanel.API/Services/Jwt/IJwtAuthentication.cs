using BankingControlPanel.Data.Models;
using System.Security.Claims;

namespace BankingControlPanel.API.Services.Jwt
{
    public interface IJwtAuthentication
    {
        Task<string> Authenticate(string email, string password, string role);  // Authenticate the user and return a token
        string GenerateToken(int id, string email, string role);                         // Generate a JWT token for the authenticated user
    }
}
