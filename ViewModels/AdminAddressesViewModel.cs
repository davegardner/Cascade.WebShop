using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Cascade.WebShop.Models;

namespace Cascade.WebShop.ViewModels
{
    public class AdminAddressesViewModel : ContentPart
    {
        public IContent InvoiceAddress { get; set; }
        public IContent ShippingAddress { get; set; }
    }
}