using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.Mvc;
using Orchard.Themes;
using Cascade.WebShop.Models;
using Cascade.WebShop.Services;
using Cascade.WebShop.ViewModels;
using System;
using Cascade.WebShop.Extensibility;

namespace Cascade.WebShop.Controllers
{
    public class SimulatedPaymentServiceProviderController : Controller
    {
        private readonly IOrchardServices _services;

        public SimulatedPaymentServiceProviderController(IOrchardServices services)
        {
            _services = services;
        }

        public ActionResult Index(string orderReference, int amount)
        {
            // Return a standard MVC PartialView instead of a ShapeResult
            return PartialView(new PaymentResult { OrderReference = orderReference, Amount = amount });
        }


        [HttpPost]
        public ActionResult Command(string command, string orderReference)
        {

            // Generate a fake payment ID
            var paymentId = new Random(Guid.NewGuid().GetHashCode()).Next(1000, 9999);

            // Redirect back to the webshop
            return RedirectToAction("PaymentResponse", "Order", new { area = "Cascade.WebShop", paymentId = paymentId, result = command, orderReference });
        }
    }
}