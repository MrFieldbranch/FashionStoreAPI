using FashionStoreAPI.DTOs;
using FashionStoreAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FashionStoreAPI.Controllers
{
    [ApiController]
    [Route("login")]
    public class LoginController : ControllerBase
    {
        private readonly LoginService _loginService;
        public LoginController(LoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var tokenResponse = await _loginService.AuthenticateAsync(request);

                if (tokenResponse == null)
                    return Unauthorized("Felaktiga inloggningsuppgifter.");

                return Ok(tokenResponse);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"Konfigurationsproblem. {ex.Message}" );
            }
            catch (Exception)
            {
                return StatusCode(500, "Problem med databasen. Vänligen försök igen.");
            }
        }
    }    
}
