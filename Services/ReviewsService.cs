using FashionStoreAPI.Data;
using FashionStoreAPI.Entities;
using FashionStoreAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FashionStoreAPI.Services
{
    public class ReviewsService
    {
        private readonly ApplicationDbContext _context;

        public ReviewsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateReviewAsync(int productId, int userId, string text)
        {
            var product = await _context.Products
                .Include(p => p.Ratings)
                .FirstOrDefaultAsync(p => p.Id == productId) ?? throw new ResourceNotFoundException("Produkten finns inte.");

            var existingRatingForProduct = product.Ratings
                .FirstOrDefault(r => r.ProductId == productId && r.UserId == userId) ?? throw new ResourceNotFoundException("Du kan inte recensera en produkt om du inte har betygsatt den.");

            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);

            if (existingReview != null)
                throw new ConflictException("Du har redan recenserat denna produkt.");
            else
            {
                var review = new Review
                {
                    ProductId = productId,
                    UserId = userId,
                    Text = text,
                    RatingId = existingRatingForProduct.Id
                };

                _context.Reviews.Add(review);
            }

            try
            {
                await _context.SaveChangesAsync();
            }            
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "22001")
            {
                throw new ArgumentException("Recensionen kan max vara 500 tecken lång."); 
            }
        }
    }
}
