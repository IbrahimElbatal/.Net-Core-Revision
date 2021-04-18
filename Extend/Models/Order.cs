using System.Collections.Generic;

namespace Extend.Models
{
    public class Order
    {
        public int Id { get; set; }
        public ICollection<CartLine> Lines { get; set; }
        public string Name { get; set; }
        public string Line1 { get; set; }
    }
}
