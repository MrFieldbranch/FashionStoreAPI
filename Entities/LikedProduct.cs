namespace FashionStoreAPI.Entities
{
    public class LikedProduct
    {
        public int UserId { get; set; }

        public int ProductId { get; set; }


        // Navigation properties
        public User? User { get; set; }

        public Product? Product { get; set; }
    }
}
