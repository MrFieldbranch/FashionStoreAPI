namespace FashionStoreAPI.DTOs
{
    public class ShoppingBasketItemResponse
    {
        public int ProductVariantId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string Size { get; set; } = string.Empty;

        public double Price { get; set; }

        //Kanske behöver ännu fler attribut senare
    }
}
