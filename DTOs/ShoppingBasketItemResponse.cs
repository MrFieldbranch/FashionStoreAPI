namespace FashionStoreAPI.DTOs
{
    public class ShoppingBasketItemResponse
    {
        public int ProductVariantId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string Size { get; set; } = string.Empty;

        public double Price { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public int Stock { get; set; }

        public string Color { get; set; } = string.Empty;

        public int Quantity { get; set; } = 1; //Antal av denna produktvariant i varukorgen

        //Kanske behöver ännu fler attribut senare
    }
}
