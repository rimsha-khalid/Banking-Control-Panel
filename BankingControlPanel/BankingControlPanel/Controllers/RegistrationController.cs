using BankingControlPanel.Model.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BankingControlPanel.Controllers
{
    public class RegistrationController : Controller
    {

        private readonly HttpClient _httpClient;
        public string URL = "https://localhost:7270/api/User/Registration/";

        public RegistrationController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        [HttpGet]
        public IActionResult Register()
        {
            var registration = new Registration();
            return View(registration);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Registration model)
        {

            if (!ModelState.IsValid)
            {
                return View(model); // Return the same view if validation fails
            }

            try
            {
                // Send registration data to the API directly from the model
                var response = await _httpClient.PostAsJsonAsync(URL, model);

                if (response.IsSuccessStatusCode)
                {
                    // Successful registration
                    TempData["SuccessMessage"] = "Registration successful. Please log in.";
                    return RedirectToAction("Login", "Login"); // Redirect to login page
                }

                // Handle failure
                TempData["ErrorMessage"] = "Registration failed. Please try again.";
                return View(model); // Return the view with the model
            }
            catch
            {
                // Handle unexpected errors
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return View(model);
            }
        }

    }

}


