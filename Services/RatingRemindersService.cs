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

            //var cutoffDate = DateTime.UtcNow.AddDays(-7);  // Tar bort denna just nu

            var reminders = await _context.RatingReminders
                    .Include(rr => rr.Product)
                    .Where(rr => rr.UserId == userId
                    && !rr.HasBeenAnswered)
                    //&& rr.CreatedAt <= cutoffDate)
                    .Select(rr => new RatingReminderResponse
                    {
                        ProductId = rr.ProductId,
                        ProductName = rr.Product.Name,
                        ProductImageUrl = rr.Product.ImageUrl
                    })
                    .ToListAsync();

            return reminders;
        }

        public async Task<bool> MarkReminderAsAnsweredAsync(int userId, int productId)
        {
            var reminder = await _context.RatingReminders
                .FirstOrDefaultAsync(rr => rr.UserId == userId && rr.ProductId == productId);

            if (reminder == null)
                return false;

            reminder.HasBeenAnswered = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task MarkAllRemindersAsAnsweredAsync(int userId)
        {
            var reminders = await _context.RatingReminders
                .Where(rr => rr.UserId == userId && !rr.HasBeenAnswered)
                .ToListAsync();

            foreach (var reminder in reminders)
            {
                reminder.HasBeenAnswered = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
