using BankingControlPanel.API.Repositories.Clients;
using BankingControlPanel.API.Repositories.Users;
using BankingControlPanel.Data.Data;
using BankingControlPanel.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static BankingControlPanel.API.Repositories.Clients.ClientRepository;

namespace BankingControlPanel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClient _client;
        public ClientController(IClient client)
        {
            _client = client;
        }
        // Get Paginated Clients
        [HttpGet("PagenatedData")]
        public async Task<ActionResult<PaginationResult>> GetClientsPagination(
            [FromQuery] int pageNum = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sort = null,
            [FromQuery] Sex? sex = null)
        {
            try
            {
                // Call the Pagination method from the repository
                var result = await _client.Pagination(pageNum, pageSize, sort, sex);

                // If the result is null, return a NotFound response
                if (result == null)
                {
                    return NotFound(new { message = "No clients found." });
                }

                // Return the paginated result
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the error (you can implement your own logging mechanism here)
                return StatusCode(500, new { message = "An error occurred while processing the request.", details = ex.Message });
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public async Task<ActionResult<List<Client>>> GetAllClient()
        {
            var response = await _client.GetAllClient();
            return Ok(response);
        }
        [Authorize(Roles = "Admin, User")]
        [HttpGet("GetClientByID/{id}")]
        public async Task<ActionResult<Client>> GetClientByID(int id)
        {
            var response = await _client.GetClientById(id);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("RegisterClient")]
        public async Task<ActionResult<Client>> AddClient(Client client)
        {
            try
            {
                // Attempt to add the client using the repository method
                var response = await _client.AddClient(client);
                return Ok(response);  // Return the created client object if successful
            }
            catch (Exception ex)
            {
                // Return a generic error message for unexpected exceptions
                return BadRequest(new { message = "Email is not assicaited to any user or belongs to Admin" });
            }
        }
        [Authorize(Roles = "Admin, User")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Client>> UpdateClient(int id, Client client)
        {
            if (id != client.ClientId)
            {
                return BadRequest();
            }
            
            var response = await _client.UpdateClient(id, client);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Client>> DeleteClient(int id)
        {
            var response = await _client.DeleteClient(id);
            if (response == null)
            {
                return NotFound();
            }
            return Ok();
        }
   
    }

}

