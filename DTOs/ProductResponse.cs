namespace FashionStoreAPI.DTOs
{
    public class ProductResponse
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string ImageUrl { get; set; }

        public double? StartPrice { get; set; }

        public string? Description { get; set; }

        public string? Color { get; set; }

        public List<ProductVariantResponse> ProductVariants { get; set; } = [];
    }
}
