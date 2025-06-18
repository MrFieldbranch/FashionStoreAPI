namespace FashionStoreAPI.DTOs
{
    public class DetailedOrderResponse
    {
        public int OrderId { get; set; }        
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public List<OrderItemResponse> Items { get; set; } = [];        
    }
}
