using FashionStoreAPI.DTOs;
using FashionStoreAPI.Exceptions;
using FashionStoreAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FashionStoreAPI.Controllers
{
    [ApiController]
    [Route("ratings")]
    public class RatingsController : ControllerBase
    {
        private readonly RatingsService _ratingsService;
        public RatingsController(RatingsService ratingsService)
        {
            _ratingsService = ratingsService;
        }

        [HttpPost("{productId:int}")]
        public async Task<IActionResult> CreateRating(int productId, [FromBody] CreateRatingRequest request)
        {
            if (request.Grade < 1 || request.Grade > 5)
                return BadRequest("Betyget måste vara mellan 1 och 5.");

            int? userId = null;

            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (roleClaim != null && roleClaim.Value == "User")
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
                    userId = parsedUserId;
            }

            if (!userId.HasValue)
                return Unauthorized();            
            
            try
            {
                await _ratingsService.CreateRatingAsync(productId, userId.Value, request.Grade);
                return NoContent();
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ConflictException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Problem med databasen: {ex.Message}");
            }
        }
    }
}
