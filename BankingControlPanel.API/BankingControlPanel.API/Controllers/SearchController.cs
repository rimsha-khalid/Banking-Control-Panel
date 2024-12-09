using BankingControlPanel.API.Repositories.Searches;
//using BankingControlPanel.API.Services.Repositories;
using BankingControlPanel.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BankingControlPanel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        // Dependency for ISearch Interface
        private readonly ISearch _search;

        // Constructor that initializes the ISearch instance
        public SearchController(ISearch search)
        {
            _search = search;
        }
         
        [Authorize(Roles = "Admin")]
        // HttpGet/Search - To search Clients from datasource
        [HttpGet]
        public async Task<IActionResult> SearchClients(string searchRecord)
        {
            // Perform search operation using provided search record
            var response = await _search.SearchClients(searchRecord);
            // Check if any clients were found
            if (!response.Any())
            {
                // Return not found response if no clients match the search criteria
                return NotFound(new { Message = "No clients found for the search criteria." });
            }
            // Return a 200 OK response with succesful search
            return Ok(response);  
        }
        
        [Authorize(Roles = "Admin")]
        // HttpGet/RecentSearches - To get last three searches stored in datasource
        [HttpGet("RecentSearches")]
        public async Task<IActionResult> GetRecentSearches()
        {
            // Retrieve recent search records
            var response = await _search.GetRecentSearches();
            // Check if any recent searches exist
            if (response == null || !response.Any())
            {
                // Return not found response if no recent searches are available
                return NotFound(new { Message = "No recent searches available." });
            }
            // Return a 200 OK response with recent searches
            return Ok(response);  
        }
    }
}
 