using BankingControlPanel.Data.Models;
using Microsoft.EntityFrameworkCore;
using BankingControlPanel.Data.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure;
using System.Security.Principal;
using System.Reflection;
using BankingControlPanel.API.Repositories.Users;
using Microsoft.IdentityModel.Claims;
using System.Web.Http.ModelBinding;

namespace BankingControlPanel.API.Repositories.Clients
{
    public class ClientRepository : IClient
    {
        private readonly BankingControlPanelDbContext _context;
        public ClientRepository(BankingControlPanelDbContext context)
        {
            _context = context;
        }
        public async Task<PaginationResult> Pagination(int pageNum, int pageSize, string? sort, Sex? sex)
        {
            var totalClientsRecord = await _context.Clients
                .CountAsync();
            var totalPages = (int)Math.Ceiling(totalClientsRecord / (double)pageSize);
            var clients = await _context.Clients
                .Include(e => e.Address)
                .Include(e => e.Account)
                .ToListAsync();

            switch (sex)
            {
                case Sex.Male:
                    clients = clients.Where(c => c.Sex == Sex.Male).Take(pageSize).ToList();
                    break;

                case Sex.Female:
                    clients = clients.Where(c => c.Sex == Sex.Female).Take(pageSize).ToList();
                    break;

                case null:
                    // No filtering by sex, so do nothing
                    break;

                default:
                    // If an invalid Filter value is passed, you could log or handle it here
                    break;
            }

            switch (sort)
            {
                case "asc":
                    clients = clients.Skip((pageNum - 1) * pageSize)
                         .Take(pageSize)
                         .OrderBy(c => c.FirstName)
                         .ToList();
                    break;

                case "desc":
                    clients = clients.Skip((pageNum - 1) * pageSize)
                         .Take(pageSize)
                         .OrderBy(c => c.FirstName)
                         .OrderByDescending(c => c.FirstName)
                         .ToList();
                    break;

                default:
                    clients = clients
                         .Skip((pageNum - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();

                    break;

            }


            //// save pagination to a database  
            //var pagination = new Pagination
            //{
            //    TotalRecords = totalClientsRecord,
            //    TotalPages = totalPages,
            //    Filter = sex.ToString()
            //};

            //// Save to database  
            //await _context.Paginations.AddAsync(pagination);
            //await _context.SaveChangesAsync();

            //Generate pagination response here

            var response = new PaginationResult
            {
                TotalRecords = totalClientsRecord,
                TotalPages = totalPages,
                CurrentPage = pageNum,
                PageSize = pageSize,
                Clients = clients
            };

            return response;

        }
        // Get all clients without pagination
        public async Task<List<Client>> GetAllClient()
        {

            var clients = await _context.Clients
                .Include(e => e.Address)
                .Include(e => e.Account)
                .ToListAsync();
            return clients; // Return list of clients
        }
        // Get client by unique Identifier
        public async Task<Client> GetClientById(int id)
        {
            var client = await _context.Clients
               .Include(e => e.Address)
               .Include(e => e.User)
               .Include(e => e.Account)
               .Where(e => e.ClientId == id)
               .FirstOrDefaultAsync();
            return client;
        }
        // Add a new client along with related entities (Address, Account)
        public async Task<Client> AddClient(Client client)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = await _context.Users
                                    .Where(u => u.Email == client.Email && u.Role != "Admin")
                                    .FirstOrDefaultAsync();

                    if (user == null)
                    {
                        throw new InvalidOperationException("The provided email is not associated with a valid user or the user is an admin.");
                    }

                    // Set the UserId of the client to the UserId of the existing user
                    client.UserId = user.UserId;

                    await _context.Clients.AddAsync(client);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return client;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new InvalidOperationException("Failed to add client: " + ex.Message);
                }
            }
        }
        public async Task<Client> UpdateClient(int id, Client client)
        {
            // Start a new transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var existingClient = await _context.Clients
                        .Include(e => e.User)        // Include User entity if needed
                        .Include(e => e.Address)      // Include Address if it's a related entity
                        .Include(e => e.Account)      // Include Account if it's related
                        .FirstOrDefaultAsync(c => c.ClientId == id);

                    if (existingClient == null)
                    {
                        // If client not found, return null or you can throw an exception
                        return null;  // Return 404 if client not found
                    }

                    // Update client details
                    existingClient.Email = client.Email;
                    existingClient.FirstName = client.FirstName;
                    existingClient.LastName = client.LastName;
                    existingClient.PersonalId = client.PersonalId;
                    existingClient.MobileNumber = client.MobileNumber;
                    existingClient.Sex = client.Sex;

                    // Update address if it exists and is provided
                    if (existingClient.Address != null && client.Address != null)
                    {
                        existingClient.Address.Street = client.Address.Street;
                        existingClient.Address.Country = client.Address.Country;
                        existingClient.Address.City = client.Address.City;
                        existingClient.Address.ZipCode = client.Address.ZipCode;
                    }

                    // Update account if it exists and is provided
                    if (existingClient.Account != null && client.Account != null)
                    {
                        var existingAccount = existingClient!.Account!.FirstOrDefault(a => a.AccountId == existingClient!.Account!.Select(e => e.AccountId).FirstOrDefault());
                        if (existingAccount != null)
                        {
                            existingAccount.AccountNumber = client!.Account!.Select(e => e.AccountNumber).FirstOrDefault();
                            existingAccount.AccountType = client!.Account!.Select(e => e.AccountType).FirstOrDefault();
                            existingAccount.Balance = client!.Account!.Select(e => e.Balance).FirstOrDefault();
                        }
                    }

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Commit the transaction if everything is successful
                    await transaction.CommitAsync();

                    // Return the updated client object
                    return existingClient;
                }
                catch (Exception ex)
                {
                    // Rollback the transaction if anything goes wrong
                    await transaction.RollbackAsync();

                    // Log the exception or handle as needed
                    throw new InvalidOperationException("An error occurred while updating the client.", ex);
                }
            }
        }

        public async Task<Client> DeleteClient(int id)
        {
            // Start a new transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Find the client by id along with related entities (User, Address, Account)
                    var client = await _context.Clients
                        .Include(e => e.User)       // Include User (foreign key)
                        .Include(e => e.Address)     // Include related Address if needed
                        .Include(e => e.Account)     // Include related Account if needed
                        .FirstOrDefaultAsync(c => c.ClientId == id);

                    // If client is not found, return null
                    if (client == null)
                    {
                        return null;  // Client not found
                    }

                    // Delete the related Address if it exists
                    if (client.Address != null)
                    {
                        _context.Addresses.Remove(client.Address);  // Remove Address
                    }

                    // Delete related Accounts if they exist
                    if (client.Account != null)
                    {
                        foreach (var account in client.Account)
                        {
                            _context.Accounts.Remove(account);  // Remove Account
                        }
                    }

                    // Delete the related User if needed
                    if (client.User != null)
                    {
                        // If you need to delete the User as well
                        _context.Users.Remove(client.User);  // Remove related User
                    }

                    // Remove the client from the Clients table
                    _context.Clients.Remove(client);

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Commit the transaction if everything is successful
                    await transaction.CommitAsync();

                    // Return the deleted client
                    return client;
                }
                catch (Exception ex)
                {
                    // Rollback the transaction if anything goes wrong
                    await transaction.RollbackAsync();

                    // Log the exception or handle as needed
                    throw new InvalidOperationException("An error occurred while deleting the client.", ex);
                }
            }
        }
    }

}

