using FashionStoreAPI.Data;
using FashionStoreAPI.DTOs;
using FashionStoreAPI.Entities;
using FashionStoreAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.ComponentModel.DataAnnotations;

namespace FashionStoreAPI.Services
{
    public class ProductVariantsService
    {
        private readonly ApplicationDbContext _context;

        public ProductVariantsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductVariantResponse> CreateNewProductVariantAsync(int productId, CreateNewProductVariantRequest request)
        {
            var existingProduct = await _context.Products
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.Id == productId)
                ?? throw new ResourceNotFoundException("Produkten finns inte.");

            var existingSize = existingProduct.ProductVariants
                .FirstOrDefault(v => v.Size == request.Size);

            if (existingSize != null)
                throw new ConflictException("Denna produkt har redan denna storlek.");

            var existingSKU = await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.SKU == request.SKU);

            if (existingSKU != null)
                throw new ConflictException("Detta SKU finns redan i databasen.");             

            var newVariant = new ProductVariant
            {
                Size = request.Size,
                SKU = request.SKU,
                Price = request.Price,
                Stock = request.Stock,
                ProductId = productId                
            };

            try
            {
                existingProduct.ProductVariants.Add(newVariant);

                await _context.SaveChangesAsync();

                return new ProductVariantResponse
                {
                    ProductVariantId = newVariant.Id,
                    Size = newVariant.Size,
                    SKU = newVariant.SKU,
                    Price = newVariant.Price,
                    Stock = newVariant.Stock,
                    ProductId = existingProduct.Id
                };
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
            {
                throw new ArgumentException("Max längd: Storlek: 15 tecken, SKU: 20 tecken."); // Testa detta sedan.
            }
        }

        public async Task<ProductVariantResponse> UpdateExistingProductVariantAsync(int productId, UpdateProductVariantRequest request)
        {
            var existingProduct = await _context.Products
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.Id == productId) ?? throw new ResourceNotFoundException("Produkten finns inte.");

            var existingVariant = existingProduct.ProductVariants
                .FirstOrDefault(v => v.Id == request.ProductVariantId) ?? throw new ResourceNotFoundException("Produktvarianten finns inte.");            

            if (request.StockChange != null)
            {
                existingVariant.Stock += request.StockChange.Value;
                if (existingVariant.Stock < 0)
                    throw new ArgumentException("Lagersaldot kan inte vara negativt.");
            }

            if (request.NewPrice != null && request.NewPrice != existingVariant.Price)
                existingVariant.Price = request.NewPrice.Value;

            await _context.SaveChangesAsync();

            return new ProductVariantResponse
            {
                ProductVariantId = existingVariant.Id,
                Size = existingVariant.Size,
                SKU = existingVariant.SKU,
                Price = existingVariant.Price,
                Stock = existingVariant.Stock,
                ProductId = existingProduct.Id
            };
        }
    }
}
