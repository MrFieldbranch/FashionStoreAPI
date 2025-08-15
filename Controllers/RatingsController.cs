using FashionStoreAPI.DTOs;
using FashionStoreAPI.Exceptions;
using FashionStoreAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ConflictException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Du har redan betygsatt denna produkt.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Problem med databasen: {ex.Message}");
            }
        }
    }
}
