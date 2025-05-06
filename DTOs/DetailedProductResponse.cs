namespace FashionStoreAPI.DTOs
{
    public class DetailedProductResponse
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string ImageUrl { get; set; }

        public required double StartPrice { get; set; }

        public required string Description { get; set; }

        public required string Color { get; set; }

        // Mer information om produkten? Antagligen inte.
    }
}
