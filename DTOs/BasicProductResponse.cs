namespace FashionStoreAPI.DTOs
{
    public class BasicProductResponse
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string ImageUrl { get; set; }

        public required double StartPrice { get; set; }
    }
}
