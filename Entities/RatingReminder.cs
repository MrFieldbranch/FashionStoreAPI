using Microsoft.EntityFrameworkCore;

namespace FashionStoreAPI.Entities
{
    [Index(nameof(ProductId), nameof(UserId), IsUnique = true)]
    public class RatingReminder
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!; // Required to avoid null reference issues, will be set by EF Core

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool HasBeenAnswered { get; set; } = false;
    }
}
