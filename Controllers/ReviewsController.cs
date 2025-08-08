using FashionStoreAPI.DTOs;
using FashionStoreAPI.Exceptions;
using FashionStoreAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FashionStoreAPI.Controllers
{
    [ApiController]
    [Route("reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewsService _reviewsService;
        public ReviewsController(ReviewsService reviewsService)
        {
            _reviewsService = reviewsService;
        }

        [HttpPost("{productId:int}")]
        public async Task<IActionResult> CreateReview(int productId, [FromBody] CreateReviewRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest("Recensionen måste ha ett innehåll.");

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
                await _reviewsService.CreateReviewAsync(productId, userId.Value, request.Text);
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
            catch (ArgumentException ex)
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
