using FashionStoreAPI.DTOs;
using FashionStoreAPI.Exceptions;
using FashionStoreAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FashionStoreAPI.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _usersService;
        public UsersController(UsersService usersService)
        {
            _usersService = usersService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("allusers")]
        public async Task<ActionResult<UserListResponse>> GetAllUsersForAdmin()
        {
            try
            {
                var userListResponse = await _usersService.GetAllUsersForAdminAsync();
                return Ok(userListResponse);
            }
            catch (Exception)
            {
                return StatusCode(500, "Problem med databasen. Vänligen försök igen.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{userid:int}")]
        public async Task<ActionResult<OrderListForUserResponse>> GetOrdersForUserByIdForAdmin(int userId)
        {
            try
            {
                var userResponse = await _usersService.GetOrdersForUserByIdForAdminAsync(userId);
                return Ok(userResponse);
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

        [Authorize(Roles = "Admin")]
        [HttpGet("{userid:int}/order/{orderid:int}")]
        public async Task<ActionResult<DetailedOrderResponse>> GetOrderByIdForAdmin(int userId, int orderId)
        {
            try
            {
                var orderResponse = await _usersService.GetOrderByIdForAdminAsync(userId, orderId);
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
    }
}
