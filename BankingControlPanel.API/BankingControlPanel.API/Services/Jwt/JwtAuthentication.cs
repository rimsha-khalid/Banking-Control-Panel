using BankingControlPanel.Data.Data;
using BankingControlPanel.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankingControlPanel.API.Services.Jwt
{
    // Service for handling JWT authentication
    public class JwtAuthentication : IJwtAuthentication
    {
        public readonly BankingControlPanelDbContext _context; // DbContext for accessing user data
        public readonly IConfiguration _configuration; // Configuration for accessing JWT settings

        // Constructor that initializes the dbcontext and configuration
        public JwtAuthentication(BankingControlPanelDbContext contex, IConfiguration configuration)
        {
            _context = contex;
            _configuration = configuration;
        }
        // Method to authenticate a user and return a JWT token
        public async Task<string> Authenticate(string email, string password, string role)
        {
            // Retrieve the user from the database based on the provided email
            var user = await _context.Users.FirstOrDefaultAsync(a => a.Email == email);

            // Check if the user exists and if the password matches
            if (user == null || user.Password != password)
            {
                throw new UnauthorizedAccessException("Invalid credentials");   // Throw exception for invalid credentials

            }

            // Check if the provided role matches the user's role
            if (!string.IsNullOrEmpty(role) && user.Role != role)
            {
                throw new UnauthorizedAccessException("Role mismatch"); // Throw exception for role mismatch
            }

            return GenerateToken(user.UserId, user.Email, user.Role); // Return token if authentication is successful
        }
        // Method to generate a JWT token for the authenticated user
        public string GenerateToken(int id, string email, string role)
        {
            // Create a list of claims to include in the token

            var claims = new List<Claim>
                {
                
                    new Claim(ClaimTypes.NameIdentifier, id.ToString()), // User's id claim
                    new Claim(ClaimTypes.Email, email),    // User's email claim
                    new Claim(ClaimTypes.Role, role),      // User's role clai
                };

            // Create a symmetric security key using the secret key from the configuration
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));

            // Set up the signing credentials using the security key and the HMAC SHA256 algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create a JWT token with the provided claims, issuer, audience, and expiration time
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],     // Set the token's issuer  
                audience: _configuration["Jwt:Audience"], // Set the token's audience  
                claims: claims,                           // Claims to include in the token
                expires: DateTime.Now.AddMinutes(30),     // Set the expiration time (e.g., 30 minutes)
                signingCredentials: creds                 // Signing credentials to secure the token
            );

            // Return the generated token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
