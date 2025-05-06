namespace FashionStoreAPI.DTOs
{
    public class UpdateProductVariantManuallyRequest
    {
        public required int ProductVariantId { get; set; }

        public int? StockChange { get; set; }

        public double? NewPrice { get; set; }
    }
}
