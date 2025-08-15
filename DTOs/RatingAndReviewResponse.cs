namespace FashionStoreAPI.DTOs
{
    public class RatingAndReviewResponse
    {
        public int RatingId { get; set; }

        public int ProductId { get; set; }

        public int Grade { get; set; } 

        public int? ReviewId { get; set; }

        public string? ReviewText { get; set; }

        public string UserFirstName { get; set; } = null!;

        public string UserLastName { get; set; } = null!;
    }
}
