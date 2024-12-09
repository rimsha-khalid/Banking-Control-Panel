using BankingControlPanel.Model.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankingControlPanel.Controllers
{
    public class UserController : Controller
    {

        private string URL = "https://localhost:7270/api/Client/";
        public string IdURL = "https://localhost:7270/api/Client/GetClientByID/";

        private readonly HttpClient _httpClient;
        public readonly IWebHostEnvironment _hostingEnvironment;

        public UserController(HttpClient httpClient, IWebHostEnvironment hostingEnvironment)
        {
            _httpClient = httpClient;
            _hostingEnvironment = hostingEnvironment;

        }
        [Route("User/{id}")]
        [HttpGet]
        public async Task<ActionResult> Index()
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
                // var userEmail = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var userId = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                // Get client data using the clientId passed to the URL
                // var response = await _httpClient.GetFromJsonAsync<Client>(URL + userId);
                var clients = await _httpClient.GetFromJsonAsync<List<Client>>(URL);
                if (clients != null)
                {
                    var filterClient = clients.Where(e => e.UserId.ToString() == userId).FirstOrDefault();
                    // Populate the view with client data
                    ViewData["ClientId"] = filterClient.ClientId;
                    ViewData["Email"] = filterClient.Email;
                    ViewData["FirstName"] = filterClient.FirstName;
                    ViewData["LastName"] = filterClient.LastName;
                    ViewData["Mobile"] = filterClient.MobileNumber;
                    ViewData["PersonalId"] = filterClient.PersonalId;
                    ViewData["Sex"] = filterClient.Sex.ToString();
                    TempData["ProfilePath"] = filterClient.ProfilePhoto;
                    ViewData["Country"] = filterClient.Address!.Country;
                    ViewData["City"] = filterClient.Address.City;
                    ViewData["Street"] = filterClient.Address.Street;
                    ViewData["ZipCode"] = filterClient.Address.ZipCode;
                    ViewBag.Accounts = filterClient.Account;
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
        // API URL to update client data




        [Route("User/UpdateProfile")]
        // GET action to load the current client's data and populate the view
        [HttpGet("{id}")]
        public async Task<ActionResult> UpdateProfile(int id)
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

        [Route("User/UpdateProfile")]
        // POST action to handle the client update form submission
        [HttpPost]
        public async Task<ActionResult> UpdateProfile(ClientRegistration registration)
        {
            int id = registration.ClientId;
            // Retrieve the JWT token from cookies
            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var role = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                // Check if the model state is valid before proceeding with the update
                if (ModelState.IsValid)
                {
                    // If no image is provided, retain the existing profile picture
                    if (registration.ProfilePhoto == null)
                    {
                        // Fetch the existing client data to retain the profile picture
                        var response = await _httpClient.GetFromJsonAsync<Client>(IdURL + id);
                        var imageUrl = Url.Content("~/ProfilePhoto/images.png");
                        if (response != null)
                        {
                            // Create a client object with updated data, keeping the existing profile image
                            var updatedClient = new Client
                            {
                                ClientId = id,
                                Email = registration.Email,
                                FirstName = registration.FirstName,
                                LastName = registration.LastName,
                                PersonalId = registration.PersonalId,
                                ProfilePhoto = response.ProfilePhoto!,  // Existing image path
                                MobileNumber = registration.MobileNumber,
                                Sex = registration.Sex,
                                Address = new Address
                                {
                                    Country = registration.Country,
                                    City = registration.City,
                                    Street = registration.Street,
                                    ZipCode = registration.ZipCode
                                },
                                Account = new List<Account>
                                {
                                    new Account
                                    {
                                        AccountNumber = registration.AccountNumber,
                                        Balance = registration.Balance,
                                        AccountType = registration.AccountType
                                    }
                                }
                            };

                            // Send the update request to the API
                            var updateResponse = await _httpClient.PutAsJsonAsync(URL + id, updatedClient);

                            // If update is successful, redirect to Admin profile page
                            if (updateResponse.IsSuccessStatusCode)
                            {
                                TempData["UpdateMessage"] = "Client updated successfully.";
                                return RedirectToAction("Index", "User"); // Redirect to Admin list
                            }
                            else
                            {
                                // If update fails, show an error message
                                ViewData["UpdateMessage"] = "Client update failed. Please try again.";
                                return RedirectToAction("UpdateProfile");
                            }
                        }
                        else
                        {
                            ViewData["Error"] = "Error fetching client details.";
                        }
                    }
                    else
                    {

                        var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "ProfilePhoto");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + registration.ProfilePhoto!.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Save the uploaded image to the server
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await registration.ProfilePhoto.CopyToAsync(fileStream);
                        }

                        // Get the URL of the uploaded image
                        var imageUrl = Url.Content("~/ProfilePhoto/" + uniqueFileName);

                        // Create the client object with updated data, including the new profile image
                        var updatedClient = new Client
                        {
                            ClientId = id,
                            Email = registration.Email,
                            FirstName = registration.FirstName,
                            LastName = registration.LastName,
                            PersonalId = registration.PersonalId,
                            ProfilePhoto = imageUrl, // Use the new uploaded image URL
                            MobileNumber = registration.MobileNumber,
                            Sex = registration.Sex,
                            Address = new Address
                            {
                                Country = registration.Country,
                                City = registration.City,
                                Street = registration.Street,
                                ZipCode = registration.ZipCode
                            },
                            Account = new List<Account>
                            {
                                new Account
                                {
                                    AccountNumber = registration.AccountNumber,
                                    Balance = registration.Balance,
                                    AccountType = registration.AccountType
                                }
                            }
                        };

                        // Send the update request to the API
                        var updateResponse = await _httpClient.PutAsJsonAsync(URL + id, updatedClient);

                        // If update is successful, redirect to User profile page
                        if (updateResponse.IsSuccessStatusCode)
                        {
                            TempData["UpdateMessage"] = "Client updated successfully.";
                            return RedirectToAction("Index", "User"); // Redirect to User Profile
                        }
                        else
                        {
                            // If update fails, show an error message
                            ViewData["UpdateMessage"] = "Client update failed. Please try again.";
                            return RedirectToAction("UpdateProfile");
                        }
                    }
                }
                else
                {
                    // If the model is not valid, show a validation error message
                    ViewData["Error"] = "Please ensure all fields are correctly filled out.";
                }
                return View();
            }
            catch (Exception ex)
            {
                // Display a general error message to the user
                ViewData["Error"] = ex.Message;
                return View();
            }
        }
    }
}
