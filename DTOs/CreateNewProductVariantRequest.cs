

namespace FashionStoreAPI.DTOs
{
    public class CreateNewProductVariantRequest
    {
        //public required int ProductId { get; set; }   Ha med ProductId som route parameter istället
        
        public required string Size { get; set; }
        
        public required string SKU { get; set; }

        public required double Price { get; set; }

        public int Stock { get; set; } = 0;        
    }
}
