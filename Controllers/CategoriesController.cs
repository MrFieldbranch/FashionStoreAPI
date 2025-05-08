using FashionStoreAPI.DTOs;
using FashionStoreAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FashionStoreAPI.Controllers
{
    [Authorize]
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
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"Fel vid hämtning av kategorier: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ett oväntat fel inträffade. {ex.Message}");
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
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"Fel vid hämtning av kategorin (inklusive produkter). {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ett oväntat fel inträffade. {ex.Message}");
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
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ett oväntat fel inträffade. {ex.Message}");
            }
        }
    }
}
