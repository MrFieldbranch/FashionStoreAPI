using FashionStoreAPI.Data;
using FashionStoreAPI.DTOs;
using FashionStoreAPI.Entities;
using FashionStoreAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.ComponentModel.DataAnnotations;

namespace FashionStoreAPI.Services
{
    public class CategoriesService
    {
        private readonly ApplicationDbContext _context;

        public CategoriesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BasicCategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
            .OrderBy(c => c.Name)
            .Select(c => new BasicCategoryResponse
            {
                Id = c.Id,
                Name = c.Name
            }).ToListAsync();

            return categories;
        }

        public async Task<DetailedCategoryResponse> GetProductsByCategoryAsync(int categoryId)
        {
            var categoryWithProducts = await _context.Categories
            .Include(c => c.Products)
                .ThenInclude(p => p.ProductVariants)
            .FirstOrDefaultAsync(c => c.Id == categoryId) ?? throw new ResourceNotFoundException("Kategorin finns inte.");


            var response = new DetailedCategoryResponse
            {
                Id = categoryWithProducts.Id,
                Name = categoryWithProducts.Name,
                ProductCount = categoryWithProducts.Products.Count,
                ProductsInCategory = categoryWithProducts.Products.Select(p => new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageUrl = p.ImageUrl,
                    StartPrice = p.ProductVariants.Count != 0 ? p.ProductVariants.Min(v => v.Price) : 0
                }).ToList()
            };

            return response;
        }

        public async Task<BasicCategoryResponse> CreateNewCategoryAsync(CreateNewCategoryRequest request)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == request.Name);

            if (existingCategory != null)
                throw new ConflictException("Kategorin finns redan.");

            var newCategory = new Category
            {
                Name = request.Name
            };

            try
            {
                _context.Categories.Add(newCategory);

                await _context.SaveChangesAsync();

                return new BasicCategoryResponse
                {
                    Id = newCategory.Id,
                    Name = newCategory.Name
                };
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
            {
                throw new ArgumentException("Namnet på kategorin kan max vara 30 tecken långt."); // Testa detta sedan.
            }            
        }
    }
}
