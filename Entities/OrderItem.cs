namespace FashionStoreAPI.Entities
{
    public class OrderItem
    {        
        public int ProductVariantId { get; set; }

        public int Quantity { get; set; } = 1;

        public double PriceAtPurchaseTime { get; set; } 

        public int OrderId { get; set; }


        // Navigation properties
        public Order? Order { get; set; }

        public ProductVariant? ProductVariant { get; set; }
       
    }
}
