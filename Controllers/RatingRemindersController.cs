using FashionStoreAPI.DTOs;
using FashionStoreAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FashionStoreAPI.Controllers
{
    [ApiController]
    [Route("ratingreminders")]
    public class RatingRemindersController: ControllerBase
    {
        private readonly RatingRemindersService _ratingRemindersService;
        public RatingRemindersController(RatingRemindersService ratingRemindersService)
        {
            _ratingRemindersService = ratingRemindersService;
        }

        [HttpGet]
        public async Task<ActionResult<List<RatingReminderResponse>>> GetRatingReminders()
        {
            int? userId = null;

            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (roleClaim != null && roleClaim.Value == "User")
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
                    userId = parsedUserId;
            }

            try
            {
                var reminders = await _ratingRemindersService.GetRatingRemindersAsync(userId);
                return Ok(reminders);
            }
            catch (Exception)
            {
                return StatusCode(500, "Problem med databasen. Vänligen försök igen.");
            }
        }

        [HttpPost("{productId}/answer")]
        public async Task<IActionResult> MarkReminderAsAnswered(int productId)
        {
            int? userId = null;

            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (roleClaim != null && roleClaim.Value == "User")
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
                    userId = parsedUserId;
            }

            if (!userId.HasValue)
                return Unauthorized();  // Kanske lägga till på andra ställen också. Kanske något meddelande inne i Unauthorized om det går?

            try
            {
                var success = await _ratingRemindersService.MarkReminderAsAnsweredAsync(userId.Value, productId);

                if (!success)
                    return NotFound("Användaren har inte med denna produkt i listan över produkter som väntar på svar.");

                return NoContent();                
            }
            catch (Exception)
            {
                return StatusCode(500, "Problem med databasen. Vänligen försök igen.");
            }
        }

        [HttpPost("answer-all")]
        public async Task<IActionResult> MarkAllRemindersAsAnswered()
        {
            int? userId = null;

            var roleClaim = User.FindFirst(ClaimTypes.Role);
            if (roleClaim != null && roleClaim.Value == "User")
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
                    userId = parsedUserId;
            }

            if (!userId.HasValue)
                return Unauthorized();  // Kanske lägga till på andra ställen också. Kanske något meddelande inne i Unauthorized om det går? Unauthorized(new { message = "Your message here" })
            try
            {
                await _ratingRemindersService.MarkAllRemindersAsAnsweredAsync(userId.Value);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "Problem med databasen. Vänligen försök igen.");
            }
        }
    }
}
