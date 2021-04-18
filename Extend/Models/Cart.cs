using System.Collections.Generic;
using System.Linq;

namespace Extend.Models
{
    public class CartLine
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
    public class Cart
    {
        private List<CartLine> lineCollection = new List<CartLine>();

        public virtual void AddItem(Product product)
        {
            var lineInCart = lineCollection
                .FirstOrDefault(l => l.Product.Id == product.Id);

            if (lineInCart == null)
                lineCollection.Add(new CartLine() { Product = product, Quantity = 1 });
            else
                lineInCart.Quantity += 1;
        }

        public virtual void RemoveLine(Product product)
        {
            var line = lineCollection.FirstOrDefault(l => l.Product.Id == product.Id);
            if (line != null)
                lineCollection.Remove(line);
        }

        public virtual double ComputeTotalValue()
        {
            return lineCollection.Sum(l => l.Product.Price * l.Quantity);
        }

        public virtual void Clear()
        {
            lineCollection.Clear();
        }

        public virtual IEnumerable<CartLine> Lines => lineCollection;
    }
}
