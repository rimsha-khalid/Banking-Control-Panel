using BankingControlPanel.API.Repositories.Accounts;
using BankingControlPanel.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankingControlPanel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccount _account;
        public AccountController(IAccount account)
        {
            _account = account;
        }
        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public async Task<ActionResult<Account>> GetAllAccount()
        {
            try
            {
                var response = await _account.GetAllAccount();
                if (response == null)
                {
                    return NotFound("No account found");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin, User")]
        [HttpGet("GetAccountById/{id}")]
        public async Task<ActionResult<Account>> GetAccountById(int id)
        {
            try
            {
                var response = await _account.GetAccountById(id);
                if (response == null)
                {
                    return NotFound();
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Account>> AddAccount(Account account)
        {
            try
            {
                var response = await _account.AddAccount(account);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Client>> UpdateAccount(int id, Account account)
        {
            try
            {
                if (account == null || id != account.AccountId)
                {
                    return BadRequest();
                }

                var response = await _account.UpdateAccount(id, account);
                if (response == null)
                {
                    return NotFound();
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Account>> DeleteAccount(int id)
        {
            try
            {
                var response = await _account.DeleteAccount(id);
                if (response == null)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
