using BankingControlPanel.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankingControlPanel.Controllers
{
    public class UpdateController : Controller
    {
        // API URL to update client data
        public string URL = "https://localhost:7270/api/Client/GetClientByID/";
        public string PostURL = "https://localhost:7270/api/Client/";


        // HttpClient for making API requests
        public readonly HttpClient _httpClient;

        // IWebHostEnvironment for handling file uploads (profile image)
        public readonly IWebHostEnvironment _hostingEnvironment;

        // Constructor to initialize HttpClient and IWebHostEnvironment
        public UpdateController(HttpClient httpClient, IWebHostEnvironment hostingEnvironment)
        {
            _httpClient = httpClient;
            _hostingEnvironment = hostingEnvironment;
        }
        [Route("Update/UpdateClient/{id}")]
        // GET action to load the current client's data and populate the view
        [HttpGet("{id}")]
        public async Task<ActionResult> UpdateClient(int id)
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
                var response = await _httpClient.GetFromJsonAsync<Client>(URL + id);

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

        [Route("Update/UpdateClient/{id}")]
        // POST action to handle the client update form submission
        [HttpPost]
        public async Task<ActionResult> UpdateClient(ClientRegistration registration)
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
                        var response = await _httpClient.GetFromJsonAsync<Client>(URL + id);
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
                                ProfilePhoto = imageUrl,  // Existing image path
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
                            var updateResponse = await _httpClient.PutAsJsonAsync(PostURL + id, updatedClient);

                            // If update is successful, redirect to Admin profile page
                            if (updateResponse.IsSuccessStatusCode)
                            {
                                TempData["UpdateMessage"] = "Client updated successfully.";
                                return RedirectToAction("Index", "Admin"); // Redirect to Admin list
                            }
                            else
                            {
                                // If update fails, show an error message
                                ViewData["UpdateMessage"] = "Client update failed. Please try again.";
                                return RedirectToAction("UpdateClient");
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
                        var updateResponse = await _httpClient.PutAsJsonAsync(PostURL + id, updatedClient);

                        // If update is successful, redirect to Admin profile page
                        if (updateResponse.IsSuccessStatusCode)
                        {
                            TempData["UpdateMessage"] = "Client updated successfully.";
                            return RedirectToAction("Index", "Admin"); // Redirect to Admin list
                        }
                        else
                        {
                            // If update fails, show an error message
                            ViewData["UpdateMessage"] = "Client update failed. Please try again.";
                            return RedirectToAction("UpdateClient");
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

//using BankingControlPanel.Model.Models;
//using Microsoft.AspNetCore.Mvc;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;

//namespace BankingControlPanel.Controllers
//{
//    public class UpdateController : Controller
//    {

//        // API URL to update client data
//        public string url = "https://localhost:7270/api/Client/";

//        // HttpClient for making API requests
//        public readonly HttpClient _httpClient;

//        // IWebHostEnvironment for handling file uploads (profile image)
//        public readonly IWebHostEnvironment _hostingEnvironment;

//        // Constructor to initialize HttpClient and IWebHostEnvironment
//        public UpdateController(HttpClient httpClient, IWebHostEnvironment hostingEnvironment)
//        {
//            _httpClient = httpClient;
//            _hostingEnvironment = hostingEnvironment;
//        }

//        // GET action to load the current client's data and populate the view
//        [HttpGet]
//        public async Task<ActionResult> Updateclient()
//        {
//            // Retrieve the JWT token from cookies
//            var token = Request.Cookies["JwtToken"];

//            // If token is not available, return Unauthorized response
//            if (string.IsNullOrEmpty(token))
//            {
//                return Unauthorized();
//            }

//            try
//            {
//                var handler = new JwtSecurityTokenHandler();
//                var jwtToken = handler.ReadJwtToken(token);

//                // Set authorization header with the token
//                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

//                // Get the user ID from the JWT token claims
//                var userId = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

//                // Retrieve client data using the user ID from the API
//                var response = await _httpClient.GetFromJsonAsync<Client>(url + userId);

//                // If client data is found, populate the view with the data
//                if (response != null)
//                {

//                    ViewData["ClientId"] = response.ClientId;
//                    ViewData["FirstName"] = response.FirstName;
//                    ViewData["LastName"] = response.LastName;
//                    ViewData["Mobile"] = response.MobileNumber;
//                    ViewData["PersonalId"] = response.PersonalId;
//                    ViewData["Sex"] = response.Sex;
//                    TempData["ProfilePath"] = response.ProfilePhoto;
//                    ViewData["Country"] = response.Address!.Country;
//                    ViewData["City"] = response.Address.City;
//                    ViewData["Street"] = response.Address.Street;
//                    ViewData["ZipCode"] = response.Address.ZipCode;
//                    ViewBag.Accounts = response.Account;
//                }
//                else
//                {
//                    // If no client data is found, show an error message
//                    ViewData["Error"] = "No client data found.";
//                }
//                return View();
//            }
//            catch (Exception ex)
//            {
//                // Error retrieving client data.
//                ViewData["Error"] = ex.Message;
//                return View();
//            }
//        }

//        // POST action to handle the client update form submission
//        [HttpPost]
//        public async Task<ActionResult> Updateclient(ClientRegistration registration)
//        {
//            await Updateclient();
//            // Retrieve the JWT token from cookies
//            var token = Request.Cookies["JwtToken"];

//            // If token is not available, return Unauthorized response
//            if (string.IsNullOrEmpty(token))
//            {
//                return Unauthorized();
//            }

//            try
//            {
//                var handler = new JwtSecurityTokenHandler();
//                var jwtToken = handler.ReadJwtToken(token);

//                // Set authorization header with the token
//                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

//                // Get the user ID and role from the JWT token claims
//                var userId = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
//                var role = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

//                // Check if the model state is valid before proceeding with the update
//                if (ModelState.IsValid)
//                {
//                    // If no image is provided, retain the existing profile picture
//                    if (registration.ProfilePhoto == null)
//                    {
//                        // Fetch the existing client data to retain the profile picture
//                        var response = await _httpClient.GetFromJsonAsync<Client>(url + userId);

//                        if (response != null)
//                        {
//                            // Create a client object with updated data, keeping the existing profile image
//                            var existClient = new Client
//                            {
//                                FirstName = registration.FirstName,
//                                LastName = registration.LastName,
//                                PersonalId = registration.PersonalId,
//                                ProfilePhoto = response.ProfilePhoto!,  // Existing image path
//                                MobileNumber = registration.MobileNumber,
//                                Sex = registration.Sex,
//                                Address = new Address
//                                {
//                                    Country = registration.Country,
//                                    City = registration.City,
//                                    Street = registration.Street,
//                                    ZipCode = registration.ZipCode
//                                },
//                                Account = new List<Account>
//                                {
//                                    new Account
//                                    {
//                                        AccountNumber = registration.AccountNumber,
//                                        Balance = registration.Balance,
//                                        AccountType = registration.AccountType
//                                    }
//                                }
//                            };

//                            // Send the update request to the API
//                            var updateResponse = await _httpClient.PutAsJsonAsync(url + userId, existClient);

//                            // If update is successful, redirect to AdminProfile
//                            if (updateResponse.IsSuccessStatusCode)
//                            {
//                                TempData["UpdateMessage"] = "Client updated successfully.";
//                                if (role == "Admin")
//                                {
//                                    return RedirectToAction("Adminprofile", "AdminProfile");
//                                }
//                                else if (role == "User")
//                                {
//                                    return RedirectToAction("UserDashboard", "UserDashBoard");
//                                }
//                            }
//                            else
//                            {
//                                // If update fails, show an error message
//                                ViewData["UpdateMessage"] = "Client update failed. Please try again.";
//                                return RedirectToAction("UpdateClient");
//                            }
//                        }
//                        else
//                        {
//                            ViewData["Error"] = "Error fetching client details.";
//                        }
//                    }
//                    else
//                    {
//                        // Handle case where a new image is uploaded

//                        // Save the uploaded image to the server
//                        var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
//                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + registration.ProfilePhoto!.FileName;
//                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

//                        // Save the uploaded image to the server
//                        using (var fileStream = new FileStream(filePath, FileMode.Create))
//                        {
//                            await registration.ProfilePhoto.CopyToAsync(fileStream);
//                        }

//                        // Generate the URL for the uploaded image
//                        var imageUrl = Url.Content("~/images/" + uniqueFileName);

//                        // Create the client object with updated data, including the new profile image
//                        var client = new Client
//                        {
//                            FirstName = registration.FirstName,
//                            LastName = registration.LastName,
//                            PersonalId = registration.PersonalId,
//                            ProfilePhoto = imageUrl, // Use the new uploaded image URL
//                            MobileNumber = registration.MobileNumber,
//                            Sex = registration.Sex,
//                            Address = new Address
//                            {
//                                Country = registration.Country,
//                                City = registration.City,
//                                Street = registration.Street,
//                                ZipCode = registration.ZipCode
//                            },
//                            Account = new List<Account>
//                            {
//                                new Account
//                                {

//                                    AccountNumber = registration.AccountNumber,
//                                    Balance = registration.Balance,
//                                    AccountType = registration.AccountType

//                                }
//                            }
//                        };

//                        // Send the update request to the API
//                        var updateResponse = await _httpClient.PutAsJsonAsync(url + userId, client);

//                        // If update is successful, redirect to AdminProfile
//                        if (updateResponse.IsSuccessStatusCode)
//                        {
//                            TempData["UpdateMessage"] = "Client updated successfully.";

//                            if (role == "Admin")
//                            {
//                                return RedirectToAction("Index", "Admin");
//                            }
//                            else if (role == "User")
//                            {
//                                return RedirectToAction("UserDashboard", "UserDashBoard");
//                            }
//                        }
//                        else
//                        {
//                            // If update fails, show an error message
//                            ViewData["UpdateMessage"] = "Client update failed. Please try again.";
//                            return RedirectToAction("UpdateClient");
//                        }
//                    }
//                }
//                else
//                {
//                    // If the model is not valid, show a validation error message
//                    ViewData["Error"] = "Please ensure all fields are correctly filled out.";
//                }
//                return View();
//            }
//            catch (Exception ex)
//            {
//                // Display a general error message to the user
//                ViewData["Error"] = ex.Message;
//                return View();
//            }

//            // Return the view with error messages or validation issues

//        }
//    }

//}

