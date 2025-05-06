namespace FashionStoreAPI.DTOs
{
    // För Admin
    public class CreateNewProductRequest
    {
        public required string Name { get; set; }

        public required string Description { get; set; }
        
        public required string ImageUrl { get; set; }

        public required string Color { get; set; }        
    }
}
