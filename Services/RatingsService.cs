using FashionStoreAPI.Data;
using FashionStoreAPI.Entities;
using FashionStoreAPI.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FashionStoreAPI.Services
{
    public class RatingsService
    {
        private readonly ApplicationDbContext _context;

        public RatingsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateRatingAsync(int productId, int userId, int grade)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId) ?? throw new ResourceNotFoundException("Produkten finns inte.");

            var existingRating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);

            if (existingRating != null)
                throw new ConflictException("Du har redan betygsatt denna produkt.");
            else
            {
                var rating = new Rating
                {
                    ProductId = productId,
                    UserId = userId,
                    Grade = grade
                };

                _context.Ratings.Add(rating);
            }

            await _context.SaveChangesAsync();
        }
    }
}
