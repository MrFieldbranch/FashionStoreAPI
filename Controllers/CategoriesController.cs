﻿using FashionStoreAPI.DTOs;
using FashionStoreAPI.Exceptions;
using FashionStoreAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FashionStoreAPI.Controllers
{    
    [ApiController]
    [Route("categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoriesService _categoriesService;
        public CategoriesController(CategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        [HttpGet("sex/{sex}/allcategories")]
        public async Task<ActionResult<List<BasicCategoryResponse>>> GetAllCategoriesBasedOnSex(string sex)
        {
            try
            {
                var categoriesBasedOnSex = await _categoriesService.GetAllCategoriesBasedOnSexAsync(sex);
                return Ok(categoriesBasedOnSex);
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

        [HttpGet("{categoryid:int}/sex/{sex}/products")]
        public async Task<ActionResult<DetailedCategoryResponse>> GetProductsByCategoryBasedOnSex(int categoryId, string sex)
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
                var categoryWithProductsBasedOnSex = await _categoriesService.GetProductsByCategoryBasedOnSexAsync(categoryId, sex, userId);

                return Ok(categoryWithProductsBasedOnSex);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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

        [HttpGet("{categoryid:int}/products")]
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
        public async Task<IActionResult> CreateNewCategory(CreateNewCategoryRequest request) // Byta till ActionResult<T> kanske. Även på andra ställen?
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
