using BankingControlPanel.API.Repositories.Users;
using BankingControlPanel.API.Services.Jwt;
using BankingControlPanel.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankingControlPanel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtAuthentication _jwtAuthentication;

        public AuthController(IJwtAuthentication jwtAuthentication)
        {
            _jwtAuthentication = jwtAuthentication;
        }
     
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest("Request body cannot be empty.");
                }

                // Validate the user's role (optional)
                if (string.IsNullOrEmpty(user.Role) || (user.Role != "Admin" && user.Role != "User"))
                {
                    return BadRequest("Invalid role. Role must be 'Admin' or 'User'.");
                }

                // Authenticate the user
                var response = await _jwtAuthentication.Authenticate(user.Email, user.Password, user.Role);
                

                if (response == null)
                { 
                    return Unauthorized("Invalid email or password.");
                }
               
                // Return the generated token directly
                return Ok(response);

            }
            catch (Exception ex)
            {
                 return StatusCode(500, ex.Message);
            }
 
        }
    }
}