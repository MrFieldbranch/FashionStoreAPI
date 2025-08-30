using FashionStoreAPI.DTOs;
using FashionStoreAPI.Exceptions;
using FashionStoreAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FashionStoreAPI.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsService _productsService;
        public ProductsController(ProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpGet("{productid:int}")]
        public async Task<ActionResult<DetailedProductResponse>> GetProduct(int productId)
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
                var product = await _productsService.GetProductAsync(productId, userId);
                return Ok(product);
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
        

        [HttpGet("most-popular")]
        public async Task<ActionResult<List<BasicProductResponse>>> GetMostPopularProductsBasedOnSex([FromQuery] string sex)
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
                var mostPopularProducts = await _productsService.GetMostPopularProductsBasedOnSexAsync(sex, userId);
                return Ok(mostPopularProducts);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                var originalMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"Problem med databasen. Vänligen försök igen. {originalMessage}");
            }

        }

        [HttpGet("best-rated")]
        public async Task<ActionResult<List<BasicProductResponse>>> GetBestRatedProductsBasedOnSex([FromQuery] string sex)
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
                var bestRatedProducts = await _productsService.GetBestRatedProductsBasedOnSexAsync(sex, userId);
                return Ok(bestRatedProducts);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                var originalMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"Problem med databasen. Vänligen försök igen. {originalMessage}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{categoryid:int}")]
        public async Task<IActionResult> CreateNewProduct(int categoryId, CreateNewProductRequest request)
        {
            try
            {
                var newProduct = await _productsService.CreateNewProductAsync(categoryId, request);

                return Created($"/products/{newProduct.Id}", newProduct);
            }
            catch (ConflictException ex)
            {
                return Conflict(ex.Message);
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

        [Authorize(Roles = "Admin")]
        [HttpPut("{productid:int}")]
        public async Task<IActionResult> UpdateExistingProduct(int productId, UpdateExistingProductRequest request)
        {
            try
            {
                var updatedProduct = await _productsService.UpdateExistingProductAsync(productId, request);
                return Ok(updatedProduct);
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ConflictException ex)
            {
                return Conflict(ex.Message);
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
