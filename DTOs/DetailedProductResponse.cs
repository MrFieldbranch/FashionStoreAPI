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
    }
}
