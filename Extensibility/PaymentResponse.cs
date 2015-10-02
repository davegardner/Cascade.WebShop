using System.Web;

namespace Cascade.WebShop.Extensibility
{
    public class PaymentResponse
    {
        public bool WillHandleResponse { get; set; }
        public PaymentResponseStatus Status { get; set; }
        public string OrderReference { get; set; }
        public string PaymentReference { get; set; }
        public string ResponseText { get; set; }
        public HttpContextBase HttpContext { get; private set; }

        public PaymentResponse(HttpContextBase httpContext)
        {
            HttpContext = httpContext;
        }
    }
}