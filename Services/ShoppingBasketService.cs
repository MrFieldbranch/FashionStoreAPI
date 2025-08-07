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

            if (variant.Stock == 0)
                throw new ArgumentException("Denna produktvariant finns inte i lager.");

            var existingItem = await _context.ShoppingBasketItems
                .FirstOrDefaultAsync(sbi => sbi.UserId == userId && sbi.ProductVariantId == request.ProductVariantId);

            if (existingItem != null)
            {
                if (existingItem.Quantity == variant.Stock)
                    throw new ArgumentException("Det finns inte tillräckligt i lager av denna storlek för denna ändring.");
                else if (existingItem.Quantity == 5)
                    throw new ArgumentException("Du kan max ha 5 ex av en produktvariant i varukorgen.");
                else
                    existingItem.Quantity++;
            }                
            else
            {
                var shoppingBasketItem = new ShoppingBasketItem
                {
                    UserId = userId,
                    ProductVariantId = request.ProductVariantId,
                    Quantity = 1
                };

                _context.ShoppingBasketItems.Add(shoppingBasketItem);
            } 
            
            await _context.SaveChangesAsync();            
        }

        //Undersök denna. De andra är väl bra? Fler metoder?
        public async Task RemoveItemFromShoppingBasketAsync(int userId, RemoveItemFromShoppingBasketRequest request)
        {
            var existingItem = await _context.ShoppingBasketItems
                .FirstOrDefaultAsync(sbi => sbi.UserId == userId && sbi.ProductVariantId == request.ProductVariantId) ?? 
                throw new ResourceNotFoundException("Denna produktvariant finns inte i din varukorg.");

            _context.ShoppingBasketItems.Remove(existingItem);
            await _context.SaveChangesAsync();
        }

        public async Task<ShoppingBasketResponse> GetShoppingBasketAsync(int userId)
        {
            var shoppingBasketItems = await _context.ShoppingBasketItems
                .Include(sbi => sbi.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .Where(sbi => sbi.UserId == userId)
                .ToListAsync();

            var totalQuantity = shoppingBasketItems.Sum(sbi => sbi.Quantity);

            var totalAmount = shoppingBasketItems
                .Sum(sbi => sbi.Quantity * sbi.ProductVariant.Price);

            var productIds = shoppingBasketItems
                .Select(sbi => sbi.ProductVariant.ProductId)
                .Distinct()
                .ToList();

            var shoppingBasketResponse = new ShoppingBasketResponse
            {
                TotalQuantity = totalQuantity,
                TotalAmount = totalAmount,
                ProductIds = productIds,
                Items = shoppingBasketItems.Select(sbi => new ShoppingBasketItemResponse
                {
                    ProductVariantId = sbi.ProductVariantId,
                    ProductName = sbi.ProductVariant.Product.Name,
                    Size = sbi.ProductVariant.Size,
                    Price = sbi.ProductVariant.Price,
                    ImageUrl = sbi.ProductVariant.Product.ImageUrl,
                    Stock = sbi.ProductVariant.Stock,
                    Color = sbi.ProductVariant.Product.Color,
                    Quantity = sbi.Quantity
                }).ToList()
            };            

            return shoppingBasketResponse;
        }

        public async Task<ShoppingBasketTotalAmountResponse> GetShoppingBasketTotalAmountAsync(int userId)
        {
            var shoppingBasketItems = await _context.ShoppingBasketItems
                .Include(sbi => sbi.ProductVariant)                
                .Where(sbi => sbi.UserId == userId)
                .ToListAsync();

            var totalAmount = shoppingBasketItems
                .Sum(sbi => sbi.Quantity * sbi.ProductVariant.Price);

            var totalamountResponse = new ShoppingBasketTotalAmountResponse
            {
                TotalAmount = totalAmount
            };

            return totalamountResponse;
        }

        public async Task ChangeQuantityAsync(int userId, int productVariantId, ChangeQuantityRequest request)
        {
            var existingItem = await _context.ShoppingBasketItems
                .Include(sbi => sbi.ProductVariant)
                .FirstOrDefaultAsync(sbi => sbi.UserId == userId && sbi.ProductVariantId == productVariantId) ?? 
                throw new ResourceNotFoundException("Denna produktvariant finns inte i din varukorg.");

            if (request.NewQuantity > existingItem.ProductVariant.Stock)            
                throw new ArgumentException("Det finns inte tillräckligt i lager av denna storlek för denna ändring.");

            if (request.NewQuantity > 5)
                throw new ArgumentException("Du kan max ha 5 ex av en produktvariant i varukorgen.");

            existingItem.Quantity = request.NewQuantity;

            await _context.SaveChangesAsync();
        }
    }
}
