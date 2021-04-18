using Extend.Models;
using Extend.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Extend.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductRepository _repository;
        private readonly Cart _cartService;

        public CartController(IProductRepository repository,
            Cart cartService)
        {
            _repository = repository;
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            var cart = _cartService;
            return View(cart);
        }
        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            var product = _repository.GetProducts()
                .SingleOrDefault(p => p.Id == productId);

            if (product == null)
                return NotFound();

            _cartService.AddItem(product);

            return RedirectToAction("Index");

        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var product = _repository.GetProducts()
                .SingleOrDefault(p => p.Id == productId);

            if (product == null)
                return NotFound();

            _cartService.RemoveLine(product);
            return RedirectToAction("Index");
        }
    }
}
