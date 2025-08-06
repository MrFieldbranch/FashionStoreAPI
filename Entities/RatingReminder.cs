using Microsoft.EntityFrameworkCore;

namespace FashionStoreAPI.Entities
{
    [Index(nameof(ProductId), nameof(UserId), IsUnique = true)]
    public class RatingReminder
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public required Product Product { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool HasBeenAnswered { get; set; } = false;
    }
}
