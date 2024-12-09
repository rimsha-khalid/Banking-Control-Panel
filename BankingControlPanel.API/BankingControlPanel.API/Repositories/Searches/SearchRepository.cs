using BankingControlPanel.Data.Data;
using BankingControlPanel.Data.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BankingControlPanel.API.Repositories.Searches
{
    public class SearchRepository : ISearch
    {   // Dependency injection of the BankingControlPanelDbContext
        private readonly BankingControlPanelDbContext _context;
        // Constructor that initializes the BankingControlPanelDbContext instance
        public SearchRepository(BankingControlPanelDbContext context)
        {
            _context = context;
        }
        public async Task<List<Client>> SearchClients(string searchRecord)
        {

            // Create a queryable source of clients
            var searchClient = _context.Clients.AsQueryable();

            // Apply search filter if search record argument is not empty
            if (!string.IsNullOrEmpty(searchRecord))
            {
                
                searchClient = searchClient
                       .Where(c => c.FirstName.ToLower().Contains(searchRecord.ToLower()) ||
                       c.LastName.ToLower().Contains(searchRecord.ToLower()) ||
                       c.PersonalId.Contains(searchRecord));
            }

            // Execute search and retrieve clients
            var clients = await searchClient
                
                .Include(e => e.Address)

                .Include(e => e.Account)
                .ToListAsync();

            // Check if search was successful
            bool isSuccessful = clients.Any();

            // Create new search record
            var search = new Search
            {
                SearchRecord = searchRecord,
                SearchAt = DateTime.UtcNow,
                IsSuccessful = isSuccessful
            };

            // Save search record to database
            await _context.Searches.AddAsync(search);
            await _context.SaveChangesAsync();
            return clients;
        }
        public async Task<List<Search>> GetRecentSearches()
        {

            // Retrieve 3 most recent searches  
            return await _context.Searches
                .OrderByDescending(s => s.SearchId)
                .Take(3)
                .ToListAsync();

        }
    }
}

