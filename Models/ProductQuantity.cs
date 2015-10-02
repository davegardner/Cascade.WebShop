using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cascade.WebShop.Models
{
    public sealed class ProductQuantity
    {
        public ProductPart ProductPart { get; set; }
        public int Quantity { get; set; }
    }
}