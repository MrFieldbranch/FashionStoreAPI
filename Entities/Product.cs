using FashionStoreAPI.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FashionStoreAPI.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Product
    {
        public int Id { get; set; }

        [MaxLength(30)]
        public required string Name { get; set; }

        public Sex ProductSex { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public required string ImageUrl { get; set; }

        [MaxLength(20)]
        public required string Color { get; set; }

        public ICollection<Category> Categories { get; set; } = [];

        public ICollection<ProductVariant> ProductVariants { get; set; } = [];

        public ICollection<Rating> Ratings { get; set; } = [];        

        public int SumOfGrades { get; set; }

        public int RatingsCount { get; set; }

        public double AverageGrade { get; set; }        
    }
}
