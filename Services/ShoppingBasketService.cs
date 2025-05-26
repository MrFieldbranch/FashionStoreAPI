using FashionStoreAPI.Data;

namespace FashionStoreAPI.Services
{
    public class ShoppingBasketService
    {
        private readonly ApplicationDbContext _context;

        public ShoppingBasketService(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
