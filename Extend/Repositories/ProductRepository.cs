using Extend.Models;
using System.Collections.Generic;
using System.Linq;

namespace Extend.Repositories
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProducts();
        IEnumerable<string> GetCategories();
    }

    public class ProductRepository : IProductRepository
    {
        private readonly IEnumerable<Product> products;
        public ProductRepository()
        {
            products = new List<Product>()
            {
                new Product() { Id = 1,Name = "ali",Price = 20d,Category = "Cat1"},
                new Product() { Id = 2,Name = "ana",Price = 20d,Category = "Cat2"},
                new Product() { Id = 3,Name = "mona",Price = 20d,Category = "Cat3"},
                new Product() { Id = 4,Name = "asmaa",Price = 20d,Category = "Cat4"},
                new Product() { Id = 5,Name = "ahmed",Price = 20d,Category = "Cat5"},
                new Product() { Id = 6,Name = "khalid",Price = 20d,Category = "Cat6"},
                new Product() { Id = 7,Name = "abdo",Price = 20d,Category = "Cat7"}
            };
        }

        public IEnumerable<Product> GetProducts()
        {
            return this.products;
        }

        public IEnumerable<string> GetCategories()
        {
            return this.products
                .Select(p => p.Category)
                .Distinct();
        }
    }
}
