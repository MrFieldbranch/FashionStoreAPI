namespace FashionStoreAPI.DTOs
{
    public class DetailedOrderResponse
    {
        public int OrderId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public List<OrderItemResponse> Items { get; set; } = [];        
    }
}
