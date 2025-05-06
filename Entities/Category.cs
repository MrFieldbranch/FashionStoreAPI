using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FashionStoreAPI.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Category
    {
        public int Id { get; set; }

        [MaxLength(30)]
        public required string Name { get; set; }

        public ICollection<Product> Products { get; set; } = [];
    }
}
