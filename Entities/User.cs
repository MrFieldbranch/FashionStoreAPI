using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FashionStoreAPI.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }

        [MaxLength(40)]
        public required string Email { get; set; }

        [MaxLength(150)]
        public required string Password { get; set; }

        [MaxLength(30)]
        public required string FirstName { get; set; }

        [MaxLength(30)]
        public required string LastName { get; set; }

        public bool IsAdmin { get; set; } = false;

        public ICollection<LikedProduct> LikedProducts { get; set; } = [];

        public ICollection<ShoppingBasketItem> ShoppingBasketItems { get; set; } = [];

        public ICollection<Order> Orders { get; set; } = [];
    }
}
