using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace FashionStoreAPI.Entities
{
    [Index(nameof(SKU), IsUnique = true)]
    [Index(nameof(ProductId), nameof(Size), IsUnique = true)]
    public class ProductVariant
    {
        public int Id { get; set; }

        [MaxLength(15)]
        public required string Size { get; set; }

        [MaxLength(20)]
        public required string SKU { get; set; }

        public required double Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Lagersaldo kan inte vara negativt.")]
        public int Stock { get; set; } = 0;  

        public int ProductId { get; set; }


        // Navigation property
        public Product? Product { get; set; }              
    }
}
