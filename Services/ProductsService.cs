using FashionStoreAPI.Data;
using FashionStoreAPI.DTOs;
using FashionStoreAPI.Entities;
using FashionStoreAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.ComponentModel.DataAnnotations;

namespace FashionStoreAPI.Services
{
    public class ProductsService
    {
        private readonly ApplicationDbContext _context;

        public ProductsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductResponse> GetProductAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.Id == productId) ?? throw new ResourceNotFoundException("Produkten finns inte.");

            var response = new ProductResponse
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
                    Id = v.Id,
                    Size = v.Size,
                    SKU = v.SKU,
                    Price = v.Price,
                    Stock = v.Stock,
                    ProductId = v.ProductId
                }).ToList()
            };

            return response;
        }

        public async Task<ProductResponse> CreateNewProductAsync(CreateNewProductRequest request)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Name == request.Name);

            if (existingProduct != null)
                throw new ConflictException("En produkt med detta namn finns redan.");

            var newProduct = new Product
            {
                Name = request.Name,
                ProductSex = request.ProductSex,
                Description = request.Description ?? "",
                ImageUrl = request.ImageUrl,
                Color = request.Color
            };

            try
            {
                _context.Products.Add(newProduct);

                await _context.SaveChangesAsync();

                return new ProductResponse
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
                throw new ArgumentException("Max längd: Namn: 30 tecken, Färg: 20 tecken, Beskrivning: 200 tecken, ImageUrl: 40 tecken."); // Testa detta sedan.
            }
        }

        public async Task<ProductResponse> UpdateExistingProductAsync(int productId, UpdateExistingProductRequest request)
        {
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId) ?? throw new ResourceNotFoundException("Produkten finns inte.");

            try
            {
                if (request.Name != null && request.Name != existingProduct.Name)
                    existingProduct.Name = request.Name;

                if (request.Description != null && request.Description != existingProduct.Description)
                    existingProduct.Description = request.Description;

                if (request.Color != null && request.Color != existingProduct.Color)
                    existingProduct.Color = request.Color;

                if (request.ImageUrl != null && request.ImageUrl != existingProduct.ImageUrl)
                    existingProduct.ImageUrl = request.ImageUrl;

                if (request.ProductSex != null && request.ProductSex != existingProduct.ProductSex)
                    existingProduct.ProductSex = request.ProductSex.Value;

                await _context.SaveChangesAsync();

                return new ProductResponse
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
                throw new ArgumentException("Max längd: Namn: 30 tecken, Färg: 20 tecken, Beskrivning: 200 tecken, ImageUrl: 40 tecken.");
            }            
        }
    }
}
