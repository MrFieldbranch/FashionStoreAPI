using FashionStoreAPI.Enums;

namespace FashionStoreAPI.DTOs
{
    public class BasicProductResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public Sex ProductSex { get; set; }
        public required string ImageUrl { get; set; }
        public double StartPrice { get; set; }
        public bool? IsLiked { get; set; } = false;
        public int RatingsCount { get; set; } = 0;
        public double AverageGrade { get; set; } = 0.0;
    }
}
