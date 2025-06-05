using FashionStoreAPI.DTOs;
using FashionStoreAPI.Exceptions;
using FashionStoreAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FashionStoreAPI.Controllers
{
    [ApiController]
    [Route("likedproducts")]
    public class LikedProductsController : ControllerBase
    {
        private readonly LikedProductsService _likedProductsService;
        public LikedProductsController(LikedProductsService likedProductsService)
        {
            _likedProductsService = likedProductsService;
        }

        [HttpPost("{productId:int}")]
        public async Task<IActionResult> AddProductToLiked(int productId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Användaren är inte inloggad.");
            }

            try
            {
                await _likedProductsService.AddProductToLikedAsync(userId, productId);
                return Ok();
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
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

        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> RemoveProductFromLiked(int productId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Användaren är inte inloggad.");
            }
            try
            {
                await _likedProductsService.RemoveProductFromLikedAsync(userId, productId);
                return Ok();
            }            
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Problem med databasen. Vänligen försök igen.");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<BasicProductResponse>>> GetLikedProducts()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Användaren är inte inloggad.");
            }
            try
            {
                var likedProducts = await _likedProductsService.GetLikedProductsAsync(userId);
                return Ok(likedProducts);
            }
            catch (Exception)
            {
                return StatusCode(500, "Problem med databasen. Vänligen försök igen.");
            }
        }
    }
}


