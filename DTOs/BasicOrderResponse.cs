namespace FashionStoreAPI.DTOs
{
    public class BasicOrderResponse
    {
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public double TotalAmount { get; set; }

        public int TotalQuantity { get; set; }
    }
}
