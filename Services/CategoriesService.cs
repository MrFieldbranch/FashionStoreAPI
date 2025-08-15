using FashionStoreAPI.Data;
using FashionStoreAPI.DTOs;
using FashionStoreAPI.Entities;
using FashionStoreAPI.Enums;
using FashionStoreAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FashionStoreAPI.Services
{
    public class CategoriesService
    {
        private readonly ApplicationDbContext _context;

        public CategoriesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BasicCategoryResponse>> GetAllCategoriesBasedOnSexAsync(string sex)
        {
            if (!Enum.TryParse<Sex>(sex, true, out var productSex))
                throw new ArgumentException("Ogiltig könstyp.");

            var categories = await _context.Categories
                .Where(c => c.Products.Any(p => p.ProductSex == productSex))
                .OrderBy(c => c.Name)
                .Select(c => new BasicCategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return categories;
        }

        public async Task<DetailedCategoryResponse> GetProductsByCategoryBasedOnSexAsync(int categoryId, string sex, int? userId)
        {
            if (!Enum.TryParse<Sex>(sex, true, out var productSex))
                throw new ArgumentException("Ogiltig könstyp.");

            var likedProductIds = userId.HasValue
                ? await _context.LikedProducts
                .Where(lp => lp.UserId == userId)
                .Select(lp => lp.ProductId)
                .ToListAsync()
                : new List<int>();

            var categoryResponse = await _context.Categories
                .Where(c => c.Id == categoryId)
                .Select(c => new DetailedCategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductsInCategory = c.Products
                    .Where(p => p.ProductSex == productSex)
                    .Select(p => new BasicProductResponse
                    {
                        Id = p.Id,
                        Name = p.Name,
                        ProductSex = p.ProductSex,
                        ImageUrl = p.ImageUrl,
                        StartPrice = p.ProductVariants.Count != 0 ? p.ProductVariants.Min(v => v.Price) : 0,
                        IsLiked = userId.HasValue && likedProductIds.Contains(p.Id),
                        RatingsCount = p.RatingsCount,
                        AverageGrade = Math.Round(p.AverageGrade, 1)
                    })
                    .ToList()
                })
                .FirstOrDefaultAsync() ?? throw new ResourceNotFoundException("Kategorin finns inte.");

            categoryResponse.ProductCount = categoryResponse.ProductsInCategory.Count;

            return categoryResponse;
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
                ProductsInCategory = categoryWithProducts.Products.Select(p => new BasicProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    ProductSex = p.ProductSex,
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
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "22001")
            {
                throw new ArgumentException("Namnet på kategorin kan max vara 30 tecken långt."); // Testa detta sedan.
            }            
        }
    }
}
