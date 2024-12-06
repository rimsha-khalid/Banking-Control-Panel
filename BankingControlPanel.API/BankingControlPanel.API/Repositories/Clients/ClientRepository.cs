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

namespace BankingControlPanel.API.Repositories.Clients
{
    public class ClientRepository : IClient
    {
        private readonly BankingControlPanelDbContext _context;
        public ClientRepository(BankingControlPanelDbContext context)
        {
            _context = context;
        }
        public async Task<PaginationResult<Client>> GetClientsAsync(
            string firstName,
            string lastName,
            string sex,
            string sortBy,
            string sortOrder,
            int pageNumber,
            int pageSize,
            string userId)
        { 
            IQueryable<Client> query = _context.Clients
               .Include(c => c.User)
               .Include(c => c.Address)
               .Include(c => c.Account);

            // Apply filtering
            if (!string.IsNullOrEmpty(firstName))
            {
                query = query.Where(c => c.FirstName.Contains(firstName));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                query = query.Where(c => c.LastName.Contains(lastName));
            }

            if (!string.IsNullOrEmpty(sex) && Enum.TryParse(sex, true, out Sex parsedSex))
            {
                query = query.Where(c => c.Sex == parsedSex);
            }

            // Apply sorting
            if (string.IsNullOrEmpty(sortBy))
            {
                // Default sort by ClientId (ascending)
                query = query.OrderBy(c => c.ClientId);
            }
            else
            {
                query = sortBy.ToLower() switch
                {
                    "id" => sortOrder.ToLower() == "asc" ? query.OrderBy(c => c.ClientId) : query.OrderByDescending(c => c.ClientId),
                    "firstname" => sortOrder.ToLower() == "asc" ? query.OrderBy(c => c.FirstName) : query.OrderByDescending(c => c.FirstName),
                    "lastname" => sortOrder.ToLower() == "asc" ? query.OrderBy(c => c.LastName) : query.OrderByDescending(c => c.LastName),
                    _ => query.OrderBy(c => c.ClientId),  // Default to sorting by ClientId
                };
            }
            // Get total record count before applying pagination (Skip and Take)
            var totalRecords = await query.CountAsync();

            // If no records match the filters, we can handle it gracefully
            if (totalRecords == 0)
            {
                return new PaginationResult<Client>
                {
                    TotalCount = 0,
                    Items = new List<Client>(), // No records to return
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    FilterFirstName = firstName,
                    FilterLastName = lastName,
                    FilterSex = sex
                };
            }
            var clients = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            // Save pagination metadata to database  
            var pagination = new Pagination
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                FilterFirstName = firstName,  // Store filter directly
                FilterLastName = lastName,    // Store filter directly
                FilterSex = sex,              // Store filter directly
                UserId = userId  // Save UserId from the authentication token
            };

            // Save to database
            await _context.Paginations.AddAsync(pagination);
            await _context.SaveChangesAsync();

            // Calculate total pages based on the total record count
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // Return the pagination result
            return new PaginationResult<Client>
            {
                TotalCount = totalRecords,
                TotalPages = totalPages, // Add total pages to the result
                Items = clients,
                PageNumber = pageNumber,
                PageSize = pageSize,
                FilterFirstName = firstName,
                FilterLastName = lastName,
                FilterSex = sex
            };
        }
        

        public async Task<List<Client>> GetAllClient(int pageNumber, int pageSize)
        {
            var clients = await _context.Clients
                         .Include(e => e.Address)
                         .Include(e => e.Account)
                         .Where(e => e.Account!.Any())
                         .Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize)
                         .ToListAsync();
            return clients;
        }
        public async Task<List<Client>> GetAllClient()
        {

            var clients = await _context.Clients
                .Include(e => e.Address)
                .Include(e => e.Account)
                .ToListAsync();
            return clients;
        }
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

        public async Task<Client> AddClient(Client client)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
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

        //public async Task<Client> UpdateClient(int id, Client client)
        //{
        //    var existingClient = await _context.Clients
        //    .Include(e => e.User)        // Include User entity if needed
        //    .Include(e => e.Address)      // Include Address if it's a related entity
        //    .Include(e => e.Account)      // Include Account if it's related
        //    .FirstOrDefaultAsync(c => c.ClientId == id);

        //    if (existingClient == null)
        //    {
        //        return null;  // Return 404 if client not found
        //    }

        //    // Update client details
        //    existingClient.Email = client.Email;
        //    existingClient.FirstName = client.FirstName;
        //    existingClient.LastName = client.LastName;
        //    existingClient.PersonalId = client.PersonalId;
        //    existingClient.MobileNumber = client.MobileNumber;
        //    existingClient.Sex = client.Sex;

        //    // Update address if it exists and is provided
        //    if (existingClient.Address != null && client.Address != null)
        //    {
        //        existingClient.Address.Street = client.Address.Street;
        //        existingClient.Address.Country = client.Address.Country;
        //        existingClient.Address.City = client.Address.City;
        //        existingClient.Address.ZipCode = client.Address.ZipCode;
        //    }

        //    // Update account if it exists and is provided
        //    if (existingClient.Account != null && client.Account != null)
        //    {
        //        var existingAccount = existingClient!.Account!.FirstOrDefault(a => a.AccountId == existingClient!.Account!.Select(e => e.AccountId).FirstOrDefault());
        //        if (existingAccount != null)
        //        {
        //            existingAccount.AccountNumber = client!.Account!.Select(e => e.AccountNumber).FirstOrDefault();
        //            existingAccount.AccountType = client!.Account!.Select(e => e.AccountType).FirstOrDefault();
        //            existingAccount.Balance = client!.Account!.Select(e => e.Balance).FirstOrDefault();
        //        }
        //    }

        //    // Save changes to the database
        //    await _context.SaveChangesAsync();
        //    return existingClient;
        //}

        //var existingClient = await _context.Clients
        //    .Include(e => e.UserId)
        //    .Include(e => e.Address)
        //    .Include(e => e.Account)
        //    .FirstOrDefaultAsync(c => c.ClientId == id);
        //existingClient.Email = client.Email;
        //existingClient.FirstName = client.FirstName;
        //existingClient.LastName = client.LastName;
        //existingClient.PersonalId = client.PersonalId;
        //existingClient.MobileNumber = client.MobileNumber;
        //existingClient.Sex = client.Sex;
        ////_context.Entry(existingClient).State = EntityState.Modified;
        //if (existingClient == null)
        //{
        //    return null;
        //}
        //else
        //{
        //    if (client.Address != null)
        //    {
        //        existingClient.Address.Street = client.Address.Street;
        //        existingClient.Address.Country = client.Address.Country;
        //        existingClient.Address.City = client.Address.City;
        //        existingClient.Address.ZipCode = client.Address.ZipCode;
        //        //_context.Entry(existingClient).State = EntityState.Modified;
        //    }
        //    var existingAccount = existingClient!.Account!.FirstOrDefault(a => a.AccountId == existingClient!.Account!.Select(e => e.AccountId).FirstOrDefault());
        //    if (existingAccount != null)
        //    {
        //        existingAccount.AccountNumber = client!.Account!.Select(e => e.AccountNumber).FirstOrDefault();
        //        existingAccount.AccountType = client!.Account!.Select(e => e.AccountType).FirstOrDefault();
        //        existingAccount.Balance = client!.Account!.Select(e => e.Balance).FirstOrDefault();
        //        //existingAccount.Email = client!.Account!.Select(e => e.Email).FirstOrDefault();
        //        //existingAccount.Password = client!.Account!.Select(e => e.Password).FirstOrDefault();
        //        //existingAccount.Role = client!.Account!.Select(e => e.Role).FirstOrDefault();
        //        //_context.Entry(existingAccount).State = EntityState.Modified;

        //    }

        //    //_context.Entry(existingClient).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();
        //    return existingClient;
        //    }
        //}
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


        //public async Task<Client> DeleteClient(int id)
        //{
        //    var client = await _context.Clients.FindAsync(id);
        //    if (client != null)
        //    {
        //        _context.Clients.Remove(client);
        //        await _context.SaveChangesAsync();
        //    }
        //    return client;
        //}
        public async Task<List<Client>> FilterClients(string filterParams)
        {
            if (string.IsNullOrWhiteSpace(filterParams))
            {
                // Return an empty list or handle the case where filterRecord is invalid
                return new List<Client>();
            }
            // Log the filter criteria to the database
            var filter = new Filter
            {
                FilterParams = filterParams,
                FilterAt = DateTime.UtcNow,
            };
            await _context.Filters.AddAsync(filter);
            await _context.SaveChangesAsync();

            var clients = await _context.Clients
                .Include(e => e.Address)
                .Include(e => e.Account)
                .Where(c => c.FirstName.ToLower().Contains(filterParams.ToLower()) ||
                       c.LastName.ToLower().Contains(filterParams.ToLower()))
                .ToListAsync();
            // Perform the filtering logic
            //var clients = await _context.Clients
            //    .Include(e => e.Address)
            //    .Include(e => e.Account)
            //    .Where(c => EF.Functions.Like(c.FirstName.ToLower(), $"%{name.ToLower()}%") ||
            //               EF.Functions.Like(c.LastName.ToLower(), $"%{name.ToLower()}%"))
            //    .ToListAsync();

            return clients;
        }
        //public async Task<List<Client>> FilterByName(string name)
        //{
        //    var clients = await _context.Clients
        //        .Include(e => e.Address)
        //        .Include(e => e.Account)
        //        .Where(c => c.FirstName.ToLower().Contains(name.ToLower())  ||
        //               c.LastName.ToLower().Contains(name.ToLower()))
        //        .ToListAsync();
        //    return clients;
        //}

        public async Task<List<Client>> GetSortedClients(string sortBy)
        {
            var sortedClients = _context.Clients
                .Include(e => e.Address)
                .Include(e => e.Account)
                .AsQueryable();

            if (sortBy == "asc")
            {
                sortedClients = sortedClients.OrderBy(c => c.FirstName);
            }
            else if (sortBy == "desc")
            {
                sortedClients = sortedClients.OrderByDescending(c => c.FirstName);
            }

            return await sortedClients.ToListAsync();
        }
        //public async Task<List<Client>> Pagination(int PageNumber, int PageSize)
        //{
        //    var clients = await Context.Clients
        //                   .Include(e => e.Address)
        //                   .Include(e => e.Account)
        //                   .Skip((PageNumber - 1) * PageSize)
        //                   .Take(PageSize)
        //                   .ToListAsync();
        //    return clients;
        //}
    }

}

