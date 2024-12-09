using BankingControlPanel.Model.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.MSIdentity.Shared;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace BankingControlPanel.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;
        public readonly IWebHostEnvironment _hostingEnvironment;

        private string URL = "https://localhost:7270/api/Client/";
        private string IdURL = "https://localhost:7270/api/Client/GetClientByID/";
        private string RegURL = "https://localhost:7270/api/Client/RegisterClient/";
        private string SearchURL = "https://localhost:7270/api/Search?searchRecord=";
        private string RecentSearchesURL = "https://localhost:7270/api/Search/RecentSearches";
        private string AccURL = "https://localhost:7270/api/Account/";


        public AdminController(HttpClient httpClient , IWebHostEnvironment hostingEnvironment)
        {
            _httpClient = httpClient;
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        public async Task<ActionResult> Index(int pageNum = 1, int pageSize = 10, string sort = null, string  sex = null)
        {
            try
            {
                // Retrieve the JWT token from cookies
                var token = Request.Cookies["JwtToken"];
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Unauthorized if token is null or empty
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized();
                }
              
                // Set Authorization header with the token for API requests
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
             

                // Construct the URL for client pagination
                string url = $"https://localhost:7270/api/Client/PagenatedData?pageNum={pageNum}&pageSize={pageSize}&sort={sort}&sex={sex}";
                var response = await _httpClient.GetFromJsonAsync<PaginationResult>(url);

                // Check if client data is available
                if (response!.Clients != null)
                {
                    await RecentSearches(RecentSearchesURL);
                    var Page = new PaginationResult
                    {
                        TotalRecords = response.TotalRecords,
                        TotalPages = response.TotalPages,
                        CurrentPage = response.CurrentPage,
                        PageSize = response.PageSize,
                        Clients = response.Clients,
                    };
                    
                    // TempData["AdminMessage"] = "Client Register By Admin";
                    return View(Page);
                }
               
                return View();
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View();
            }
        }
        [Route("Admin/UserDetails/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult> UserDetails(int id)
        {

            try
            {
                // Set authorization header with the token (Admin should be logged in)
                var token = Request.Cookies["JwtToken"];
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized();
                }

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // Get client data using the clientId passed to the URL
                var response = await _httpClient.GetFromJsonAsync<Client>(IdURL + id);

                if (response != null)
                {
                    // Populate the view with client data
                    ViewData["ClientId"] = response.ClientId;
                    ViewData["Email"] = response.Email;
                    ViewData["FirstName"] = response.FirstName;
                    ViewData["LastName"] = response.LastName;
                    ViewData["Mobile"] = response.MobileNumber;
                    ViewData["PersonalId"] = response.PersonalId;
                    ViewData["Sex"] = response.Sex.ToString();
                    TempData["ProfilePath"] = response.ProfilePhoto;
                    ViewData["Country"] = response.Address!.Country;
                    ViewData["City"] = response.Address.City;
                    ViewData["Street"] = response.Address.Street;
                    ViewData["ZipCode"] = response.Address.ZipCode;
                    ViewBag.Accounts = response.Account;
                }
                else
                {
                    ViewData["Error"] = "No client data found.";
                }
                return View();
            }
            catch (Exception ex)
            {
                // Error retrieving client data.
                ViewData["Error"] = ex.Message;
                return View();
            }

        }

        [HttpGet]
        // GET: Admin/CreateClient
        public async Task<IActionResult> Create()
        {
            return View();

        }

        [HttpPost("AddClient")]
        public async Task<IActionResult> Create(ClientRegistration clientData)
        {
            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "No JWT token found in cookies.";
                return RedirectToAction("Login", "Login");
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Set the Authorization header with the Bearer token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (!ModelState.IsValid)
            {
                return View(clientData); // Show the form again with validation errors
            }
            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "ProfilePhoto");
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + clientData.ProfilePhoto!.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save the uploaded image to the server
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await clientData.ProfilePhoto.CopyToAsync(fileStream);
            }

            // Get the URL of the uploaded image
            var imageUrl = Url.Content("~/ProfilePhoto/" + uniqueFileName);

            

            var client = new Client
            {
                Email = clientData.Email,
                FirstName = clientData.FirstName,
                LastName = clientData.LastName,
                PersonalId = clientData.PersonalId,
                ProfilePhoto = imageUrl,
                MobileNumber = clientData.MobileNumber,
                Sex = clientData.Sex,
                Address = new Address
                {
                    Country = clientData.Country,
                    City = clientData.City,
                    Street = clientData.Street,
                    ZipCode = clientData.ZipCode
                },
                Account = new List<Account>
                {
                    new Account
                    {
                        AccountNumber = clientData.AccountNumber,
                        Balance = clientData.Balance,
                        AccountType = clientData.AccountType
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(RegURL, client);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Registration failed: {errorContent}");
                return View(clientData); // Show the form again with an error message
            }
            else
            {
                TempData["SuccessMessage"] = "Client registered successfully!";
                return RedirectToAction("Index"); // Redirect to success page after successful registration
            }
        }
        [HttpGet]
        public async Task<IActionResult> Search(string searchRecord)
        {
            try
            {

                // Set authorization header with the token (Admin should be logged in)
                var token = Request.Cookies["JwtToken"];
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized();
                }

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetFromJsonAsync<List<Client>>(SearchURL + searchRecord);
                if (response != null)
                {
                    var filteredPagination = new PaginationResult
                    {
                        TotalRecords = response.Count,
                        TotalPages = 1,
                        CurrentPage = 1,
                        PageSize = response.Count,
                        Clients = response.Where(e => e.Address != null)
                          .Select(e => new Client
                          {
                              ClientId = e.ClientId,
                              FirstName = e.FirstName,
                              LastName = e.LastName,
                              PersonalId = e.PersonalId,
                              ProfilePhoto = e.ProfilePhoto,
                              MobileNumber = e.MobileNumber,
                              Sex = e.Sex,
                              Address = new Address
                              {
                                  Country = e.Address!.Country,
                                  City = e.Address.City,
                                  Street = e.Address.Street,
                                  ZipCode = e.Address.ZipCode
                              },
                              Account = e.Account!
                                  .Select(account => new Account
                                  {
                                      AccountNumber = account.AccountNumber,
                                      Balance = account.Balance,
                                      AccountType = account.AccountType,
                                  }).ToList()
                          }).ToList()

                    };
                    await RecentSearches(RecentSearchesURL);
                    return View("Index", filteredPagination);
                }
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("SearchError", "ErrorDialogue");
            }
        }
        public async Task<ActionResult<Search>> RecentSearches(string RecentSearchesURL)
        {
            try
            {
                // Set authorization header with the token (Admin should be logged in)
                var token = Request.Cookies["JwtToken"];
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized();
                }

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // Get client data using the clientId passed to the URL
                var response = await _httpClient.GetFromJsonAsync<List<Search>>(RecentSearchesURL);
                if (response != null && response.Count > 0)
                {
                    ViewData["SearchHistory"] = response; 
                }
                return View(Index);
            }
            catch (Exception ex)
            {
                // Error retrieving client data.
                ViewData["Error"] = ex.Message;
                return View();
            } 
        }
    }
}



