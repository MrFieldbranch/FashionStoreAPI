namespace FashionStoreAPI.DTOs
{
    public class DetailedCategoryResponse
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public int ProductCount { get; set; }

        public List<BasicProductResponse> ProductsInCategory { get; set; } = [];
    }
}
