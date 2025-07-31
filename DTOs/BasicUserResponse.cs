namespace FashionStoreAPI.DTOs
{
    public class BasicUserResponse
    {
        public int UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public int OrderCount { get; set; }

        public double TotalOrderValueForUser { get; set; }
    }
}
