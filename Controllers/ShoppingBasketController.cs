using FashionStoreAPI.DTOs;
using FashionStoreAPI.Entities;
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
        public async Task<ActionResult<ShoppingBasketResponse>> GetShoppingBasket()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Användaren är inte inloggad.");
            }

            try
            {
                var basket = await _shoppingBasketService.GetShoppingBasketAsync(userId);
                return Ok(basket);
            }
            catch (Exception)
            {
                return StatusCode(500, "Problem med databasen. Vänligen försök igen.");
            }

        }

        [HttpGet("items/totalprice")]
        public async Task<ActionResult<ShoppingBasketTotalAmountResponse>> GetShoppingBasketTotalAmount()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Användaren är inte inloggad.");
            }

            try
            {
                var totalAmount = await _shoppingBasketService.GetShoppingBasketTotalAmountAsync(userId);
                return Ok(totalAmount);
            }
            catch (Exception)
            {
                return StatusCode(500, "Problem med databasen. Vänligen försök igen.");
            }
        }

        [HttpPut("items/{productvariantid}/quantity")]
        public async Task<IActionResult> ChangeQuantity(int productvariantid, ChangeQuantityRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Användaren är inte inloggad.");
            }

            try
            {
                await _shoppingBasketService.ChangeQuantityAsync(userId, productvariantid, request);
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
    }
}
