using Microsoft.EntityFrameworkCore;

namespace FashionStoreAPI.Entities
{
    [Index(nameof(ProductId), nameof(UserId), IsUnique = true)]
    public class Rating
    {
        public int Id { get; set; }

        public int Grade { get; set; } // 1-5

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public int ProductId { get; set; }
        
        public Review? Review { get; set; } = null;
    }
}
