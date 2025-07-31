namespace FashionStoreAPI.DTOs
{
    public class OrderListForUserResponse
    {
        public int UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public List<BasicOrderResponse> Orders { get; set; } = [];
    }
}
