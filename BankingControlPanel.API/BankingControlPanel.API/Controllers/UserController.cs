using BankingControlPanel.API.Repositories.Users;
using BankingControlPanel.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankingControlPanel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // Dependency injection of the IUser interface
        private readonly IUser _user;
        // Constructor that initializes the IUser instance
        public UserController(IUser user)
        {
           _user = user;  
        }
        // Allow anonymous access to this endpoint
        [AllowAnonymous]
        // HttpPost/Registration - POST method for user registration
        [HttpPost("Registration")]
        public async Task<ActionResult<User>> RegisterUser(User user)
        {
            try
            {
                // Call the RegisterUser method from the IUser interface
                var response = await _user.RegisterUser(user);
                // Return a 200 OK response with the registered user
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Exception handling return a 500 Internal Server Error response with the exception message
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
