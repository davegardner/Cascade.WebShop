using System.Web.Mvc;
using Cascade.WebShop.Models;

namespace Cascade.WebShop.Extensibility
{
    public class PaymentRequest
    {
        public OrderRecord Order { get; private set; }
        public bool WillHandlePayment { get; set; }
        public ActionResult ActionResult { get; set; }

        public PaymentRequest(OrderRecord order)
        {
            Order = order;
        }
    }
}