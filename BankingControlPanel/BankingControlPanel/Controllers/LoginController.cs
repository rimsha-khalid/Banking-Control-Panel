using BankingControlPanel.Model.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.MSIdentity.Shared;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;

namespace BankingControlPanel.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;
        public string URL = "https://localhost:7270/api/Auth/login";

        public LoginController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<Login>> Login(Login model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync<Login>(URL, model);

                var token = await response.Content.ReadAsStringAsync();
                Response.Cookies.Append("JwtToken", token, new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddMinutes(30)
                });
                if (token != null)
                {
                    var handler = new JwtSecurityTokenHandler();
                    token = Uri.UnescapeDataString(token);  // Decode URL-encoded string
                    var jsonToken = handler.ReadJwtToken(token) as JwtSecurityToken;
                    // var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                    var role = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                    if (role == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (role == "User")
                    {
                        return RedirectToAction("Index", "User");
                    }
                }

                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("LoginError", "ErrorDialogue");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWToken");
            return RedirectToAction("Login");
        }
    }
}
     