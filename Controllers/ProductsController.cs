using FashionStoreAPI.DTOs;
using FashionStoreAPI.Exceptions;
using FashionStoreAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("{productId:int}")]
        public async Task<ActionResult<ProductResponse>> GetProduct(int productId)
        {
            try
            {
                var product = await _productsService.GetProductAsync(productId);
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

        [Authorize(Roles = "Admin")]
        [HttpPost("{categoryId:int}")]
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
        [HttpPut("{productId:int}")]
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
