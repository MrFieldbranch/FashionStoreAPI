using FashionStoreAPI.Data;

namespace FashionStoreAPI.Services
{
    public class OrdersService
    {
        private readonly ApplicationDbContext _context;

        public OrdersService(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
