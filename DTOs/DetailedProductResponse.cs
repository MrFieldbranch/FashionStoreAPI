using FashionStoreAPI.Enums;

namespace FashionStoreAPI.DTOs
{
    public class DetailedProductResponse
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public Sex ProductSex { get; set; }

        public required string ImageUrl { get; set; }

        public double StartPrice { get; set; }

        public required string Description { get; set; }

        public required string Color { get; set; }

        public List<ProductVariantResponse> ProductVariants { get; set; } = [];

        public bool? IsLiked { get; set; } = false;

        public int RatingsCount { get; set; } = 0;

        public double AverageGrade { get; set; } = 0.0;

        public List<RatingAndReviewResponse> RatingsAndReviews { get; set; } = [];
    }
}
