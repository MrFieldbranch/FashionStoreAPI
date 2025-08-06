using FashionStoreAPI.Data;
using FashionStoreAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FashionStoreAPI.Services
{
    public class RatingRemindersService
    {
        private readonly ApplicationDbContext _context;

        public RatingRemindersService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RatingReminderResponse>> GetRatingRemindersAsync(int? userId)
        {
            if (!userId.HasValue)
                return new List<RatingReminderResponse>();

            var cutoffDate = DateTime.UtcNow.AddDays(-7);

            var reminders = await _context.RatingReminders
                    .Include(rr => rr.Product)
                    .Where(rr => rr.UserId == userId
                    && !rr.HasBeenAnswered
                    && rr.CreatedAt <= cutoffDate)
                    .Select(rr => new RatingReminderResponse
                    {
                        ProductId = rr.ProductId,
                        ProductName = rr.Product.Name,
                        ProductImageUrl = rr.Product.ImageUrl
                    })
                    .ToListAsync();

            return reminders;
        }
    }
}
