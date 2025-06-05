using FashionStoreAPI.Data;
using FashionStoreAPI.DTOs;
using FashionStoreAPI.Entities;
using FashionStoreAPI.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FashionStoreAPI.Services
{
    public class ShoppingBasketService
    {
        private readonly ApplicationDbContext _context;

        public ShoppingBasketService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddItemToShoppingBasketAsync(int userId, AddItemToShoppingBasketRequest request)
        {
            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(pv => pv.Id == request.ProductVariantId) ?? throw new ResourceNotFoundException("Produktvarianten hittades inte.");

            var existingItem = await _context.ShoppingBasketItems
                .FirstOrDefaultAsync(sbi => sbi.UserId == userId && sbi.ProductVariantId == request.ProductVariantId);

            if (existingItem != null)
                throw new ArgumentException("Denna produktvariant finns redan i din varukorg.");

            if (variant.Stock == 0)
                throw new ArgumentException("Denna produktvariant finns inte i lager.");

            var shoppingBasketItem = new ShoppingBasketItem
            {
                UserId = userId,
                ProductVariantId = request.ProductVariantId,
            };

            _context.ShoppingBasketItems.Add(shoppingBasketItem);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemFromShoppingBasketAsync(int userId, RemoveItemFromShoppingBasketRequest request)
        {
            var existingItem = await _context.ShoppingBasketItems
                .FirstOrDefaultAsync(sbi => sbi.UserId == userId && sbi.ProductVariantId == request.ProductVariantId) ?? 
                throw new ResourceNotFoundException("Denna produktvariant finns inte i din varukorg.");

            _context.ShoppingBasketItems.Remove(existingItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ShoppingBasketItemResponse>> GetShoppingBasketItemsAsync(int userId)
        {
            var shoppingBasketItems = await _context.ShoppingBasketItems
                .Include(sbi => sbi.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .Where(sbi => sbi.UserId == userId)
                .ToListAsync();

            var shoppingBasketItemsResponse = shoppingBasketItems.Select(sbi => new ShoppingBasketItemResponse
            {
                ProductVariantId = sbi.ProductVariantId,
                ProductName = sbi.ProductVariant.Product.Name,
                Size = sbi.ProductVariant.Size,
                Price = sbi.ProductVariant.Price,
            }).ToList();

            return shoppingBasketItemsResponse;
        }
    }
}
