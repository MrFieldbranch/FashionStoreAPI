using FashionStoreAPI.Data;
using FashionStoreAPI.DTOs;
using FashionStoreAPI.Entities;
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
            try
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
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Ett fel inträffade när kategorierna skulle hämtas från databasen.", ex);
            }
            catch (NpgsqlException ex) // Specific for PostgreSQL
            {
                throw new InvalidOperationException("Databasanslutningen misslyckades.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ett oväntat fel inträffade.", ex);
            }
        }

        public async Task<DetailedCategoryResponse> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                var categoryWithProducts = await _context.Categories
                .Include(c => c.Products)
                    .ThenInclude(p => p.ProductVariants)
                .FirstOrDefaultAsync(c => c.Id == categoryId) ?? throw new ArgumentException("Kategorin finns inte.");


                var response = new DetailedCategoryResponse
                {
                    Id = categoryWithProducts.Id,
                    Name = categoryWithProducts.Name,
                    ProductCount = categoryWithProducts.Products.Count,
                    ProductsInCategory = categoryWithProducts.Products.Select(p => new BasicProductResponse
                    {
                        Id = p.Id,
                        Name = p.Name,
                        ImageUrl = p.ImageUrl,
                        StartPrice = p.ProductVariants.Count != 0 ? p.ProductVariants.Min(v => v.Price) : 0
                    }).ToList()
                };

                return response;
            }
            catch (InvalidOperationException ex) 
            {
                throw new InvalidOperationException("Ett fel inträffade när kategorin skulle hämtas från databasen.", ex);
            }
            catch (NpgsqlException ex) // Specific for PostgreSQL
            {
                throw new InvalidOperationException("Databasanslutningen misslyckades.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ett oväntat fel inträffade.", ex);
            }
        }

        public async Task<BasicCategoryResponse> CreateNewCategoryAsync(CreateNewCategoryRequest request)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == request.Name);

            if (existingCategory != null)
                throw new ArgumentException("Kategorin finns redan.");

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
            catch (ValidationException ex)
            {
                throw new ArgumentException("Namnet på kategorin kan max vara 30 tecken långt.", ex); // Testa detta sedan.
            }
            catch (DbUpdateException ex)
            {
                throw new ArgumentException("Ett fel inträffade när kategorin skulle sparas i databasen.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ett oväntat fel inträffade.", ex);
            }
        }
    }
}
