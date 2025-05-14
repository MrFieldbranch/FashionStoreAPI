using FashionStoreAPI.Enums;

namespace FashionStoreAPI.DTOs
{
    public class UpdateExistingProductRequest
    {
        public string? Name { get; set; }

        public Sex? ProductSex { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Color { get; set; }
    }
}
