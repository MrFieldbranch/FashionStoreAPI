namespace FashionStoreAPI.DTOs
{
    public class UpdateProductVariantRequest
    {
        public required int ProductVariantId { get; set; }

        public int? StockChange { get; set; }

        public double? NewPrice { get; set; }
    }
}
