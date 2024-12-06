using BankingControlPanel.API.Repositories.Clients;
using BankingControlPanel.API.Repositories.Users;
using BankingControlPanel.Data.Data;
using BankingControlPanel.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllClients")]
        public async Task<IActionResult> GetClients(
         [FromQuery] string firstName = "",
         [FromQuery] string lastName = "",
         [FromQuery] string sex = "",
         [FromQuery] string sortBy = "ClientId",
         [FromQuery] string sortOrder = "asc",
         [FromQuery] int pageNumber = 1,
         [FromQuery] int pageSize = 10)
        {
            try
            {
                // Extract the UserId from the JWT token(current user's claims)
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User is not authenticated");
                }
                var response = await _client.GetClientsAsync(firstName, lastName, sex, sortBy, sortOrder, pageNumber, pageSize, userId);
                if (response == null)
                {
                    return NotFound("No client found");
                }
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<List<Client>>> GetAllClient()
        {
            var response = await _client.GetAllClient();
            return Ok(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet ("Pagination")]
        public async Task<IActionResult> GetAllClient(int pageNumber, int pageSize)
        {
            try
            {
                var response = await _client.GetAllClient(pageNumber, pageSize);
                if (response == null)
                {
                    return NotFound("No client found");
                }
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
          
        }
        [Authorize(Roles = "Admin")]
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
            var response = await _client.AddClient(client);
            return Ok(response);
        }
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpGet("GetFilteredClients")]
        public async Task<IActionResult> FilterClients(string filterParams)
        {
            var response = await _client.FilterClients(filterParams);
            if (response == null || response.Count == 0)
            {
                return NotFound("No client found with this name");
            }
            return Ok(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("Sort")]
        public async Task<IActionResult> SortClients([FromQuery] string sortBy = "asc")
        {
            if (sortBy != "asc" && sortBy != "desc")
            {
                return BadRequest("Invalid sort order. Use 'asc' for ascending or 'desc' for descending.");
            }
            var response = await _client.GetSortedClients(sortBy);

            return Ok(response);
        }
      
        //[HttpGet("GetPaginatedClients")]
        //public async Task<IActionResult> Pagination(int pageNumber, int pageSize)
        //{
        //    try
        //    {
        //        var response = await _client.Pagination(pageNumber, pageSize);
        //        if (response == null)
        //        {
        //            return NotFound("No clients found.");
        //        }
        //        return Ok(response);

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}
    }

}

