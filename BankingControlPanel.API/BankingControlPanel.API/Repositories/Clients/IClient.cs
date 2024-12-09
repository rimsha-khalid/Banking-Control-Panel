using BankingControlPanel.Data.Models;
using System.Linq.Dynamic.Core;

namespace BankingControlPanel.API.Repositories.Clients
{
    public interface IClient
    {
        Task<PaginationResult> Pagination(int pageNum, int PageSize, string? sort, Sex? sex) ;  // Get all clients with pagination
        Task<List<Client>> GetAllClient();
        Task<Client> GetClientById(int id);
        Task<Client> AddClient(Client client);
        Task<Client> UpdateClient(int id, Client client);
        Task<Client> DeleteClient(int id);
    }
}
