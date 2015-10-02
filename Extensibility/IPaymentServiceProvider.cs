using Orchard.Events;

namespace Cascade.WebShop.Extensibility
{
    public interface IPaymentServiceProvider : IEventHandler
    {
        void RequestPayment(PaymentRequest e);
        void ProcessResponse(PaymentResponse e);
    }
}