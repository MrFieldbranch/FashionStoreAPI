using FashionStoreAPI.DTOs;
using FashionStoreAPI.Exceptions;
using FashionStoreAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FashionStoreAPI.Controllers
{    
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoriesService _categoriesService;
        public CategoriesController(CategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        [HttpGet("allcategories")] // Alla kategorier, utan några produkter
        public async Task<ActionResult<List<BasicCategoryResponse>>> GetAllCategories()
        {
            try
            {
                var categories = await _categoriesService.GetAllCategoriesAsync();

                return Ok(categories);
            }
            catch (Exception)
            {
                return StatusCode(500, "Problem med databasen. Vänligen försök igen.");
            }
        }

        [HttpGet("{categoryId:int}/products")]
        public async Task<ActionResult<DetailedCategoryResponse>> GetProductsByCategory(int categoryId)
        {
            try
            {
                var categoryWithProducts = await _categoriesService.GetProductsByCategoryAsync(categoryId);

                return Ok(categoryWithProducts);
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
        [HttpPost]
        public async Task<IActionResult> CreateNewCategory(CreateNewCategoryRequest request)
        {
            try
            {
                var newCategory = await _categoriesService.CreateNewCategoryAsync(request);

                return Created($"/categories/{newCategory.Id}", newCategory);
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
