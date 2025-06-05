using FashionStoreAPI.DTOs;
using FashionStoreAPI.Exceptions;
using FashionStoreAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FashionStoreAPI.Controllers
{
    [ApiController]
    [Route("shoppingbasket")]
    public class ShoppingBasketController : ControllerBase
    {
        private readonly ShoppingBasketService _shoppingBasketService;
        public ShoppingBasketController(ShoppingBasketService shoppingBasketService)
        {
            _shoppingBasketService = shoppingBasketService;
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItemToShoppingBasket(AddItemToShoppingBasketRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Användaren är inte inloggad.");
            }
            try
            {
                await _shoppingBasketService.AddItemToShoppingBasketAsync(userId, request);
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

        [HttpDelete("items")]
        public async Task<IActionResult> RemoveItemFromShoppingBasket(RemoveItemFromShoppingBasketRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Användaren är inte inloggad.");
            }
            try
            {
                await _shoppingBasketService.RemoveItemFromShoppingBasketAsync(userId, request);
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

        [HttpGet("items")]
        public async Task<ActionResult<List<ShoppingBasketItemResponse>>> GetShoppingBasketItems()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Användaren är inte inloggad.");
            }

            try
            {
                var items = await _shoppingBasketService.GetShoppingBasketItemsAsync(userId);
                return Ok(items);
            }
            catch (Exception)
            {
                return StatusCode(500, "Problem med databasen. Vänligen försök igen.");
            }

        }
    }
}
