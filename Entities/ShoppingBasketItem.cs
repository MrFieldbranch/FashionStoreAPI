namespace FashionStoreAPI.Entities
{
    public class ShoppingBasketItem
    {
        public int UserId { get; set; }

        public int ProductVariantId { get; set; }

        public int Quantity { get; set; } = 1;


        // Navigation properties
        public User User { get; set; } = null!;

        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
