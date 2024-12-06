using BankingControlPanel.Data.Models;
using System.Linq.Dynamic.Core;

namespace BankingControlPanel.API.Repositories.Clients
{
    public interface IClient
    {
        Task<PaginationResult<Client>> GetClientsAsync(
           string firstName,
           string lastName,
           string sex,
           string sortBy,
           string sortOrder,
           int pageNumber,
           int pageSize,
           string userId);
        Task<List<Client>> GetAllClient(int pageNumber, int pageSize);
        Task<List<Client>> GetAllClient();
        Task<Client> GetClientById(int id);
        Task<Client> AddClient(Client client);
        Task<Client> UpdateClient(int id, Client client);
        Task<Client> DeleteClient(int id);
        Task<List<Client>> FilterClients(string filterParams);
        Task<List<Client>> GetSortedClients(string sortBy);
        // Task<List<Client>> Pagination(int PageNumber, int PageSize);
    }
}
