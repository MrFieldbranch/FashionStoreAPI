namespace FashionStoreAPI.DTOs
{
    public class OrderItemResponse
    { 
        //public int OrderItemId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public double PriceAtPurchaseTime { get; set; }
        public string ImageUrl { get; set; } = string.Empty;        
        public string Color { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;        
    }
}
