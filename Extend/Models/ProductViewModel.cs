using Extend.Utilities;
using System.Collections.Generic;

namespace Extend.Models
{
    public class ProductViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public PageInfo PageInfo { get; set; }
        public string CurrentCategory { get; set; }
    }
}
