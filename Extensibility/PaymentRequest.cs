using System.Web.Mvc;
using Cascade.WebShop.Models;

namespace Cascade.WebShop.Extensibility
{
    public class PaymentRequest
    {
        public OrderPart Order { get; private set; }
        public bool WillHandlePayment { get; set; }
        public ActionResult ActionResult { get; set; }

        public PaymentRequest(OrderPart order)
        {
            Order = order;
        }
    }
}