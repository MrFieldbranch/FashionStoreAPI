using FashionStoreAPI.Data;
using FashionStoreAPI.DTOs;
using FashionStoreAPI.Entities;
using FashionStoreAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == productId)
                ?? throw new ResourceNotFoundException("Produkten finns inte.");

            var existingSize = existingProduct.ProductVariants
                .FirstOrDefault(v => v.Size == request.Size);

            if (existingSize != null)
                throw new ConflictException("Denna produkt har redan denna storlek.");

            var sexPart = existingProduct.ProductSex == Enums.Sex.Unisex ? "U" : existingProduct.ProductSex == Enums.Sex.Male ? "M" : "F";

            var categoryPart = string.Join(",", existingProduct.Categories
                .Select(c => c.Id)
                .OrderBy(id => id));            

            var productPart = existingProduct.Id.ToString();

            var sizePart = request.Size;

            var newSKU = $"{sexPart}-{categoryPart}-{productPart}-{sizePart}";

            var existingSKU = await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.SKU == newSKU);

            if (existingSKU != null)
                throw new ConflictException("Detta SKU finns redan i databasen.");            

            if (request.Stock < 0)
                throw new ArgumentException("Lagersaldot kan inte vara negativt.");

            var newVariant = new ProductVariant
            {
                Size = request.Size,
                SKU = newSKU,
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
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "22001")
            {
                throw new ArgumentException("Max längd: Storlek: 15 tecken, SKU: 30 tecken."); // Testa detta sedan.
            }
        }

        public async Task<ProductVariantResponse> UpdateExistingProductVariantAsync(int productId, UpdateProductVariantRequest request)
        {
            var existingProduct = await _context.Products
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.Id == productId) ?? throw new ResourceNotFoundException("Produkten finns inte.");

            var existingVariant = existingProduct.ProductVariants
                .FirstOrDefault(v => v.Id == request.ProductVariantId) ?? throw new ResourceNotFoundException("Produktvarianten finns inte.");            

            if (request.StockChange != 0)
            {
                existingVariant.Stock += request.StockChange;
                if (existingVariant.Stock < 0)
                    throw new ArgumentException("Lagersaldot kan inte vara negativt.");
            }

            if (request.NewPrice != existingVariant.Price)
                existingVariant.Price = request.NewPrice;

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
