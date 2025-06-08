using FashionStoreAPI.Data;
using FashionStoreAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace FashionStoreAPI.Services
{
    public class OrdersService
    {
        private readonly ApplicationDbContext _context;

        public OrdersService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateOrderAsync(int userId)
        {
            var shoppingBasketItems = await _context.ShoppingBasketItems
                .Include(sbi => sbi.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .Where(sbi => sbi.UserId == userId)
                .ToListAsync();

            if (shoppingBasketItems.Count == 0)
                throw new ArgumentException("Din varukorg är tom. Lägg till produkter innan du gör en beställning.");

            var stockIssue = shoppingBasketItems
                .FirstOrDefault(sbi => sbi.Quantity > sbi.ProductVariant.Stock);

            if (stockIssue != null)
                throw new InvalidOperationException($"Lagret räcker inte till för produkten {stockIssue.ProductVariant.Product.Name} " +
                    $"i storlek {stockIssue.ProductVariant.Size}. Det finns bara {stockIssue.ProductVariant.Stock} kvar.");

            var newOrder = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = shoppingBasketItems.Sum(sbi => sbi.ProductVariant.Price * sbi.Quantity),
                OrderItems = shoppingBasketItems.Select(sbi => new OrderItem
                {
                    ProductVariantId = sbi.ProductVariantId,
                    Quantity = sbi.Quantity,
                    PriceAtPurchaseTime = sbi.ProductVariant.Price
                }).ToList()
            };

            _context.Orders.Add(newOrder);

            foreach (var sbi in shoppingBasketItems)
            {
                sbi.ProductVariant.Stock -= sbi.Quantity;                
            }

            _context.ShoppingBasketItems.RemoveRange(shoppingBasketItems);            

            var productIdsToRemove = shoppingBasketItems
                .Select(sbi => sbi.ProductVariant.ProductId)
                .Distinct()
                .ToList();

            var likedProductsToRemove = await _context.LikedProducts
                .Where(lp => lp.UserId == userId && productIdsToRemove.Contains(lp.ProductId))
                .ToListAsync();

            _context.LikedProducts.RemoveRange(likedProductsToRemove);

            await _context.SaveChangesAsync();
        }
    }
}
