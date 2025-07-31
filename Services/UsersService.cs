using FashionStoreAPI.Data;
using FashionStoreAPI.DTOs;
using FashionStoreAPI.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FashionStoreAPI.Services
{
    public class UsersService
    {
        private readonly ApplicationDbContext _context;

        public UsersService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserListResponse> GetAllUsersForAdminAsync()
        {
            var users = await _context.Users
                .Include(u => u.Orders)
                .Where(u => u.IsAdmin == false)
                .ToListAsync();

            var userListResponse = new UserListResponse
            {
                TotalNumberOfUsers = users.Count,
                Users = users.Select(u => new BasicUserResponse
                {
                    UserId = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    OrderCount = u.Orders.Count,
                    TotalOrderValueForUser = u.Orders.Sum(o => o.TotalAmount)
                }).ToList()
            };

            return userListResponse;
        }

        public async Task<OrderListForUserResponse> GetOrdersForUserByIdForAdminAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Orders)
                .ThenInclude(o => o.OrderItems)
                .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new ResourceNotFoundException("Användaren finns inte.");

            var orderListResponse = new OrderListForUserResponse
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Orders = user.Orders.Select(o => new BasicOrderResponse
                {
                    OrderId = o.Id,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    TotalQuantity = o.OrderItems.Sum(oi => oi.Quantity)
                }).ToList()
            };

            return orderListResponse;
        }

        public async Task<DetailedOrderResponse> GetOrderByIdForAdminAsync(int userId, int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId)
                ?? throw new ResourceNotFoundException("Antingen så finns inte ordern, eller så tillhör inte denna order denna användaren.");

            var orderResponse = new DetailedOrderResponse
            {
                OrderId = order.Id,
                FirstName = order.User.FirstName,
                LastName = order.User.LastName,
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
    }
}
