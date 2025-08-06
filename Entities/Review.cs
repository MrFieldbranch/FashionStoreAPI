using Microsoft.EntityFrameworkCore;

namespace FashionStoreAPI.Entities
{
    [Index(nameof(ProductId), nameof(UserId), IsUnique = true)]
    public class Review
    {
        public int Id { get; set; }

        public required string Text { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }        

        public int RatingId { get; set; }

        public Rating Rating { get; set; } = null!;
    }
}
