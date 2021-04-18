using Extend.Models;
using Extend.Repositories;
using Extend.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Extend.Controllers
{
    public class ProductController : Controller
    {

        private readonly IProductRepository _repository;

        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index(string category, int page = 1)
        {
            //            return new ViewResult()
            //            {
            //                ViewData = new ViewDataDictionary(
            //                        new EmptyModelMetadataProvider(),
            //                        new ModelStateDictionary())
            //                { Model = "" }
            //            };
            var list = _repository.GetProducts()
                .Where(p => category == null ||
                            p.Category
                                .ToLower()
                                .Contains(category.ToLower()));

            var pageInfo = new PageInfo()
            {
                TotalItems = list.Count(),
                ItemsPerPage = 3,
                CurrentPage = page

            };
            var model = new ProductViewModel()
            {
                Products = list
                    .OrderBy(e => e.Id)
                    .Skip((page - 1) * pageInfo.ItemsPerPage)
                    .Take(pageInfo.ItemsPerPage),
                PageInfo = pageInfo,
                CurrentCategory = category
            };
            return View(model);
        }
    }
}
