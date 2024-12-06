using BankingControlPanel.Data.Models;

namespace BankingControlPanel.API.Repositories.Searches
{
    public interface ISearch
    {
        Task<List<Client>> SearchClients(string searchRecord); // Searches for clients based on a search criteria
        Task<List<Search>> GetRecentSearches(); // Retrieves the recent three searches
    }
}
