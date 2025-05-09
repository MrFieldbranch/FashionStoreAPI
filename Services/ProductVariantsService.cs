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

            var duplicateVariant = await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.ProductId == productId && v.Size == request.Size || v.SKU == request.SKU);

            if (duplicateVariant != null)
            {
                if (duplicateVariant.Size == request.Size)
                    throw new ConflictException("En variant med denna storlek finns redan.");

                if (duplicateVariant.SKU == request.SKU)
                    throw new ConflictException("En variant med detta SKU finns redan.");
            }

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
                    Id = newVariant.Id,
                    Size = newVariant.Size,
                    SKU = newVariant.SKU,
                    Price = newVariant.Price,
                    Stock = newVariant.Stock,
                    ProductId = newVariant.ProductId
                };
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
            {
                throw new ArgumentException("Max längd: Storlek: 15 tecken, SKU: 20 tecken."); // Testa detta sedan. Kanske lägga till att Stock inte får vara negativ.
            }
        }

        public async Task<ProductVariantResponse> UpdateExistingProductVariantAsync(UpdateProductVariantRequest request)
        {
            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.Id == request.ProductVariantId)
                ?? throw new ResourceNotFoundException("Produktvarianten finns inte.");

            if (request.StockChange != null)
            {
                variant.Stock += request.StockChange.Value;
                if (variant.Stock < 0)
                    throw new ArgumentException("Lagersaldot kan inte vara negativt.");
            }

            if (request.NewPrice != null && request.NewPrice != variant.Price)
                variant.Price = request.NewPrice.Value;

            await _context.SaveChangesAsync();

            return new ProductVariantResponse
            {
                Id = variant.Id,
                Size = variant.Size,
                SKU = variant.SKU,
                Price = variant.Price,
                Stock = variant.Stock,
                ProductId = variant.ProductId
            };
        }
    }
}
