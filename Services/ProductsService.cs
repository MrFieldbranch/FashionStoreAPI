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
                ProductVariants = product.ProductVariants.Select(v => new ProductVariantResponse
                {
                    ProductVariantId = v.Id,
                    Size = v.Size,
                    SKU = v.SKU,
                    Price = v.Price,
                    Stock = v.Stock,
                    ProductId = product.Id
                }).ToList(),
                IsLiked = isLiked
            };

            return response;
        }

        public async Task<List<BasicProductResponse>> GetMostPopularProductsBasedOnSexAsync(string sex, int? userId)
        {
            if (!Enum.TryParse<Sex>(sex, true, out var productSex))
                throw new ArgumentException("Ogiltig könstyp.");

            var likedProductIds = userId.HasValue
                ? await _context.LikedProducts
                .Where(lp => lp.UserId == userId)
                .Select(lp => lp.ProductId)
                .ToListAsync()
                : new List<int>();

            var mostPopularProducts = await _context.OrderItems
                .Include(oi => oi.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .Where(oi => oi.ProductVariant.Product.ProductSex == productSex)
                .GroupBy(oi => oi.ProductVariant.Product)
                .Select(g => new
                {
                    Product = g.Key,
                    TotalQuantitySold = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(8)
                .Select(p => new BasicProductResponse 
                {
                    Id = p.Product.Id,
                    Name = p.Product.Name,
                    ProductSex = p.Product.ProductSex,
                    ImageUrl = p.Product.ImageUrl,
                    StartPrice = p.Product.ProductVariants.Min(v => v.Price),
                    IsLiked = likedProductIds.Contains(p.Product.Id)
                })
                .ToListAsync();

            return mostPopularProducts;
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
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
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
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
            {
                throw new ArgumentException("Max längd: Namn: 30 tecken, Färg: 20 tecken, Beskrivning: 200 tecken, ImageUrl: 200 tecken.");
            }            
        }
    }
}
