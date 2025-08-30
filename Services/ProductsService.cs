using FashionStoreAPI.Data;
using FashionStoreAPI.DTOs;
using FashionStoreAPI.Entities;
using FashionStoreAPI.Enums;
using FashionStoreAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FashionStoreAPI.Services
{
    public class ProductsService
    {
        private readonly ApplicationDbContext _context;

        public ProductsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DetailedProductResponse> GetProductAsync(int productId, int? userId)
        {
            var product = await _context.Products
                .Include(p => p.ProductVariants)
                .Include(p => p.Ratings)
                .ThenInclude(r => r.Review)
                .Include(p => p.Ratings)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == productId) ?? throw new ResourceNotFoundException("Produkten finns inte.");

            bool isLiked = false;

            if (userId.HasValue)
            {
                isLiked = await _context.LikedProducts
                    .AnyAsync(lp => lp.UserId == userId && lp.ProductId == productId);
            }

            var response = new DetailedProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                ProductSex = product.ProductSex,
                ImageUrl = product.ImageUrl,
                Description = product.Description ?? "",
                Color = product.Color,
                StartPrice = product.ProductVariants.Count != 0 ? product.ProductVariants.Min(v => v.Price) : 0,
                ProductVariants = SortProductVariantsBySize(product.ProductVariants)
                .Select(v => new ProductVariantResponse
                {
                    ProductVariantId = v.Id,
                    Size = v.Size,
                    SKU = v.SKU,
                    Price = v.Price,
                    Stock = v.Stock,
                    ProductId = product.Id
                }).ToList(),
                IsLiked = isLiked,
                RatingsCount = product.RatingsCount,
                AverageGrade = Math.Round(product.AverageGrade, 1),
                RatingsAndReviews = product.Ratings
                .Select(r => new RatingAndReviewResponse
                {
                    RatingId = r.Id,
                    ProductId = r.ProductId,
                    Grade = r.Grade,
                    ReviewId = r.Review?.Id,
                    ReviewText = r.Review?.Text ?? null,
                    UserFirstName = r.User.FirstName,
                    UserLastName = r.User.LastName
                }).ToList()
            };

            return response;
        }        

        public async Task<List<BasicProductResponse>> GetMostPopularProductsBasedOnSexAsync(string sex, int? userId)
        {
            try
            {
                if (!Enum.TryParse<Sex>(sex, true, out var productSex))
                    throw new ArgumentException("Ogiltig könstyp.");

                var likedProductIds = userId.HasValue
                    ? await _context.LikedProducts
                    .Where(lp => lp.UserId == userId)
                    .Select(lp => lp.ProductId)
                    .ToListAsync()
                    : new List<int>();

                var topProductIds = await _context.OrderItems
                    .Where(oi => oi.ProductVariant.Product.ProductSex == productSex)
                    .GroupBy(oi => oi.ProductVariant.ProductId)
                    .Select(group => new
                    {
                        ProductId = group.Key,
                        TotalQuantitySold = group.Sum(oi => oi.Quantity)
                    })
                    .OrderByDescending(x => x.TotalQuantitySold)
                    .Take(8)
                    .ToListAsync();

                var productIds = topProductIds.Select(x => x.ProductId).ToList();

                var products = await _context.Products
                    .Include(p => p.ProductVariants)
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                var result = products
                    .Where(p => p.ProductVariants.Count != 0)
                    .Select(p => new BasicProductResponse
                    {
                        Id = p.Id,
                        Name = p.Name,
                        ProductSex = p.ProductSex,
                        ImageUrl = p.ImageUrl,
                        StartPrice = p.ProductVariants.Min(v => v.Price),
                        IsLiked = likedProductIds.Contains(p.Id),
                        RatingsCount = p.RatingsCount,
                        AverageGrade = Math.Round(p.AverageGrade, 1)
                    })
                    .ToList();


                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Fel", ex);
            }
        }

        public async Task<List<BasicProductResponse>> GetBestRatedProductsBasedOnSexAsync(string sex, int? userId)
        {
            try
            {
                if (!Enum.TryParse<Sex>(sex, true, out var productSex))
                    throw new ArgumentException("Ogiltig könstyp.");

                var likedProductIds = userId.HasValue
                    ? await _context.LikedProducts
                    .Where(lp => lp.UserId == userId)
                    .Select(lp => lp.ProductId)
                    .ToListAsync()
                    : new List<int>();

                var bestRatedProducts = await _context.Products
                    .Include(p => p.ProductVariants)
                    .Where(p => p.ProductSex == productSex && p.RatingsCount > 0)
                    .OrderByDescending(p => p.AverageGrade)
                    .ThenByDescending(p => p.RatingsCount)
                    .Take(8)
                    .ToListAsync();

                var result = bestRatedProducts
                    .Where(p => p.ProductVariants.Count != 0)
                    .Select(p => new BasicProductResponse
                    {
                        Id = p.Id,
                        Name = p.Name,
                        ProductSex = p.ProductSex,
                        ImageUrl = p.ImageUrl,
                        StartPrice = p.ProductVariants.Min(v => v.Price),
                        IsLiked = likedProductIds.Contains(p.Id),
                        RatingsCount = p.RatingsCount,
                        AverageGrade = Math.Round(p.AverageGrade, 1)
                    })
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Fel", ex);
            }
        }

        public async Task<DetailedProductResponse> CreateNewProductAsync(int categoryId, CreateNewProductRequest request)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Name == request.Name);

            if (existingProduct != null)
                throw new ConflictException("En produkt med detta namn finns redan.");

            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId) ?? throw new ResourceNotFoundException("Kategorin finns inte.");

            var newProduct = new Product
            {
                Name = request.Name,
                ProductSex = request.ProductSex,
                Description = string.IsNullOrEmpty(request.Description) ? null : request.Description,
                ImageUrl = request.ImageUrl,
                Color = request.Color
            };

            newProduct.Categories.Add(existingCategory);

            try
            {
                _context.Products.Add(newProduct);

                await _context.SaveChangesAsync();

                return new DetailedProductResponse
                {
                    Id = newProduct.Id,
                    Name = newProduct.Name,
                    ProductSex = newProduct.ProductSex,
                    ImageUrl = newProduct.ImageUrl,
                    Description = newProduct.Description ?? "",
                    Color = newProduct.Color
                };
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "22001")
            {
                throw new ArgumentException("Max längd: Namn: 30 tecken, Färg: 20 tecken, Beskrivning: 200 tecken, ImageUrl: 200 tecken."); // Testa detta sedan.
            }
        }

        public async Task<DetailedProductResponse> UpdateExistingProductAsync(int productId, UpdateExistingProductRequest request)
        {
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId) ?? throw new ResourceNotFoundException("Produkten finns inte.");

            try
            {
                if (request.Name != existingProduct.Name)
                {
                    var existingProductWithSameName = await _context.Products
                        .FirstOrDefaultAsync(p => p.Name == request.Name);
                    if (existingProductWithSameName != null)
                        throw new ConflictException("En produkt med detta namn finns redan.");
                    else
                        existingProduct.Name = request.Name;
                }

                if (request.Description != existingProduct.Description)
                    existingProduct.Description = request.Description;

                if (request.Color != existingProduct.Color)
                    existingProduct.Color = request.Color;

                if (request.ImageUrl != existingProduct.ImageUrl)
                    existingProduct.ImageUrl = request.ImageUrl;

                if (request.ProductSex != existingProduct.ProductSex)
                    existingProduct.ProductSex = request.ProductSex;

                await _context.SaveChangesAsync();

                return new DetailedProductResponse
                {
                    Id = existingProduct.Id,
                    Name = existingProduct.Name,
                    ProductSex = existingProduct.ProductSex,
                    ImageUrl = existingProduct.ImageUrl,
                    Description = existingProduct.Description ?? "",
                    Color = existingProduct.Color
                };
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "22001")
            {
                throw new ArgumentException("Max längd: Namn: 30 tecken, Färg: 20 tecken, Beskrivning: 200 tecken, ImageUrl: 200 tecken.");
            }
        }

        private List<ProductVariant> SortProductVariantsBySize(IEnumerable<ProductVariant> variants)
        {
            var sizeOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["XS"] = 0,
                ["S"] = 1,
                ["M"] = 2,
                ["L"] = 3,
                ["XL"] = 4,
                ["XXL"] = 5,

                ["34"] = 10,
                ["35"] = 11,
                ["36"] = 12,
                ["37"] = 13,
                ["38"] = 14,
                ["39"] = 15,
                ["40"] = 16,
                ["41"] = 17,
                ["42"] = 18,
                ["43"] = 19,
                ["44"] = 20,
                ["45"] = 21,
                ["46"] = 22,
                ["47"] = 23,

                ["OneSize"] = 100
            };

            return variants
                .OrderBy(v => sizeOrder.TryGetValue(v.Size, out var priority) ? priority : int.MaxValue)
                .ToList();
        }
    }
}
