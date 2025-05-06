namespace FashionStoreAPI.DTOs
{
    // För Admin
    public class AddProductToCategoryRequest
    {
        public required int ProductId { get; set; }
        public required int CategoryId { get; set; }
    }
}
