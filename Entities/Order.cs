namespace FashionStoreAPI.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public double TotalAmount { get; set; } 

        public ICollection<OrderItem> OrderItems { get; set; } = [];


        // Navigation property
        public User? User { get; set; } 
    }
}
