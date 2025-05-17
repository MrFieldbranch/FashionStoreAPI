using FashionStoreAPI.DTOs;
using FashionStoreAPI.Entities;
using FashionStoreAPI.Exceptions;
using FashionStoreAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FashionStoreAPI.Controllers
{
    [ApiController]    
    [Route("products/{productId}/productvariants")]
    public class ProductVariantsController : ControllerBase
    {
        private readonly ProductVariantsService _productVariantsService;
        public ProductVariantsController(ProductVariantsService productVariantsService)
        {
            _productVariantsService = productVariantsService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateNewProductVariant(int productId, CreateNewProductVariantRequest request)
        {
            try
            {
                var newVariant = await _productVariantsService.CreateNewProductVariantAsync(productId, request);

                return Created($"/products/{productId}/productvariants/{newVariant.ProductVariantId}", newVariant); 
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

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateExistingProductVariant(int productId, UpdateProductVariantRequest request)
        {
            try
            {
                var updatedProductVariant = await _productVariantsService.UpdateExistingProductVariantAsync(productId, request);
                return Ok(updatedProductVariant); 
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
