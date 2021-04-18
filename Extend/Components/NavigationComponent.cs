using Extend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Extend.Components
{
    public class NavigationComponent : ViewComponent
    {
        private readonly IProductRepository _repository;

        public NavigationComponent(IProductRepository repository)
        {
            _repository = repository;
        }
        public IViewComponentResult Invoke()
        {
            //using QueryString
            //HttpContext.Request
            //.Query["category"]
            //  .FirstOrDefault() ?? ""
            ViewBag.CurrentCategory = RouteData.Values["category"] ?? "";
            return View(_repository.GetCategories());
        }
    }
}
