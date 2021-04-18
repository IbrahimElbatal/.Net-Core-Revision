using Extend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Extend.Components
{
    public class CartSummery : ViewComponent
    {
        private readonly Cart _cartService;

        public CartSummery(Cart cartService)
        {
            _cartService = cartService;
        }

        public IViewComponentResult Invoke()
        {
            return View(_cartService);
        }
    }
}
