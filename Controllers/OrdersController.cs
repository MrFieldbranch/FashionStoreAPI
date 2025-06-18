using FashionStoreAPI.DTOs;
using FashionStoreAPI.Exceptions;
using FashionStoreAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FashionStoreAPI.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersService _ordersService;
        public OrdersController(OrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        [HttpPost]
        public async Task<ActionResult<DetailedOrderResponse>> CreateOrder()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Användaren är inte inloggad.");
            }

            try
            {
                var orderResponse = await _ordersService.CreateOrderAsync(userId);
                return CreatedAtAction(nameof(GetOrderById), new { orderId = orderResponse.OrderId }, orderResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Problem med databasen. Vänligen försök igen.");
            }
        }

        [HttpGet("{orderId:int}")]
        public async Task<ActionResult<DetailedOrderResponse>> GetOrderById(int orderId)
        {
            try
            {
                var orderResponse = await _ordersService.GetOrderByIdAsync(orderId);
                return Ok(orderResponse);
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

        [HttpGet("allorders")]
        public async Task<ActionResult<List<BasicOrderResponse>>> GetAllOrdersForUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Användaren är inte inloggad.");
            }

            try
            {
                var allOrdersForUser = await _ordersService.GetAllOrdersForUserAsync(userId);
                return Ok(allOrdersForUser);
            }
            catch (Exception)
            {
                return StatusCode(500, "Problem med databasen. Vänligen försök igen.");
            }
        }
    }
}
