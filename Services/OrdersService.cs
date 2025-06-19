using FashionStoreAPI.Data;
using FashionStoreAPI.DTOs;
using FashionStoreAPI.Entities;
using FashionStoreAPI.Exceptions;
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

        public async Task<DetailedOrderResponse> CreateOrderAsync(int userId)
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

            var orderResponse = new DetailedOrderResponse
            {
                OrderId = newOrder.Id,
                OrderDate = newOrder.OrderDate,
                TotalAmount = newOrder.TotalAmount,
                Items = newOrder.OrderItems.Select(oi => new OrderItemResponse
                {
                    ProductVariantId = oi.ProductVariantId,
                    ProductName = oi.ProductVariant.Product.Name,
                    Size = oi.ProductVariant.Size,
                    PriceAtPurchaseTime = oi.PriceAtPurchaseTime,
                    ImageUrl = oi.ProductVariant.Product.ImageUrl,
                    Color = oi.ProductVariant.Product.Color,
                    Quantity = oi.Quantity
                }).ToList()
            };

            return orderResponse;
        }

        public async Task<DetailedOrderResponse> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId) ?? throw new ResourceNotFoundException("Ordern finns inte.");

            var orderResponse = new DetailedOrderResponse
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(oi => new OrderItemResponse
                {
                    ProductVariantId = oi.ProductVariantId,
                    ProductName = oi.ProductVariant.Product.Name,
                    Size = oi.ProductVariant.Size,
                    PriceAtPurchaseTime = oi.PriceAtPurchaseTime,
                    ImageUrl = oi.ProductVariant.Product.ImageUrl,
                    Color = oi.ProductVariant.Product.Color,
                    Quantity = oi.Quantity
                }).ToList()
            };

            return orderResponse;
        }

        public async Task<List<BasicOrderResponse>> GetAllOrdersForUserAsync(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var basicOrderList = orders.Select(o => new BasicOrderResponse
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                TotalQuantity = o.OrderItems.Sum(oi => oi.Quantity)
            }).ToList();

            return basicOrderList;
        }
    }
}
