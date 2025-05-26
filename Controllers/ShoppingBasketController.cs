using FashionStoreAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FashionStoreAPI.Controllers
{
    [ApiController]
    [Route("shoppingbasket")]
    public class ShoppingBasketController : ControllerBase
    {
        private readonly ShoppingBasketService _shoppingBasketService;
        public ShoppingBasketController(ShoppingBasketService shoppingBasketService)
        {
            _shoppingBasketService = shoppingBasketService;
        }
    }
}
