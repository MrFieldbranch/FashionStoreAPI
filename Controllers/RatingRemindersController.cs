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
    }
}
