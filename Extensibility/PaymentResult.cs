using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cascade.WebShop.Extensibility
{
    public class PaymentResult
    {
        public string OrderReference { get; set; }
        public int Amount { get; set; }
    }
}