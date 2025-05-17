namespace FashionStoreAPI.DTOs
{
    public class ProductVariantResponse
    {
        public int ProductVariantId { get; set; }

        public required string Size { get; set; }

        public required string SKU { get; set; }

        public required double Price { get; set; }

        public required int Stock { get; set; }        

        public required int ProductId { get; set; }
    }
}
