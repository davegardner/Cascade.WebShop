using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cascade.WebShop.Settings
{
    public enum ProductMode {Product, Booking};

    public class ProductSettings
    {
        public ProductMode Mode { get; set; }
        public IEnumerable<dynamic> AvailableModes { get; set; }
    }
}
