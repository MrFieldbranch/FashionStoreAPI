﻿using FashionStoreAPI.DTOs;
using FashionStoreAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FashionStoreAPI.Controllers
{
    [ApiController]
    [Route("registration")]
    public class RegistrationController : ControllerBase
    {
        private readonly RegistrationService _registrationService;
        public RegistrationController(RegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewUser(CreateNewUserRequest request)
        {
            try
            {
                bool newUserResponse = await _registrationService.RegisterNewUserAsync(request);

                if (!newUserResponse)
                    return Conflict("Det finns redan en användare med denna email registrerad.");

                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Problem med databasen. Vänligen försök igen.");
            }
        }
    }
    
}
