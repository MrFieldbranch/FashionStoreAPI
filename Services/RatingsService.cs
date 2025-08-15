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
            if (grade < 1 || grade > 5)
                throw new ArgumentException("Betyget måste vara mellan 1 och 5.");

            var productExists = await _context.Products.AnyAsync(p => p.Id == productId);

            if (!productExists)
                throw new ResourceNotFoundException("Produkten finns inte.");

            var alreadyRated = await _context.Ratings.AnyAsync(r => r.ProductId == productId && r.UserId == userId);

            if (alreadyRated)
                throw new ConflictException("Du har redan betygsatt denna produkt.");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            _context.Ratings.Add(new Rating
            {
                ProductId = productId,
                UserId = userId,
                Grade = grade
            });

            await _context.SaveChangesAsync();

            var affected = await _context.Products
                .Where(p => p.Id == productId)
                .ExecuteUpdateAsync(update => update
                .SetProperty(p => p.SumOfGrades, p => p.SumOfGrades + grade)
                .SetProperty(p => p.RatingsCount, p => p.RatingsCount + 1)
                .SetProperty(p => p.AverageGrade,
                p => ((double)(p.SumOfGrades + grade)) / (p.RatingsCount + 1)
                )
            );

            if (affected != 1)
                throw new InvalidOperationException("Misslyckades att uppdatera produktens genomsnittsbetyg.");

            await transaction.CommitAsync();
        }
    }
}
