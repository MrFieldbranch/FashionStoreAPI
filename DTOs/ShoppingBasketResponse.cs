namespace FashionStoreAPI.DTOs
{
    public class ShoppingBasketResponse
    {
        public List<ShoppingBasketItemResponse> Items { get; set; } = [];

        public double TotalAmount { get; set; } = 0;

        public int TotalQuantity { get; set; } = 0;
    }
}
