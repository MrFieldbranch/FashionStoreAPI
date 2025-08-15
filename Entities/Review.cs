using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FashionStoreAPI.Entities
{    
    public class Review
    {
        public int Id { get; set; }

        [MaxLength(500)]
        public required string Text { get; set; }              

        public int RatingId { get; set; }

        public Rating Rating { get; set; } = null!;
    }
}
