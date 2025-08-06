namespace FashionStoreAPI.DTOs
{
    public class RatingReminderResponse
    {
        public int ProductId { get; set; }

        public required string ProductName { get; set; }

        public required string ProductImageUrl { get; set; }
    }
}
