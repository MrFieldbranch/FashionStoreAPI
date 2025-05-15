namespace FashionStoreAPI.DTOs
{
    // För Admin
    public class AddExistingProductToCategoryRequest
    {
        public required int ProductId { get; set; }
        public required int CategoryId { get; set; }
    }
}
