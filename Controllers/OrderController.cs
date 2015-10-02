using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Cascade.WebShop.Extensibility;
using Cascade.WebShop.Models;
using Cascade.WebShop.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Messaging.Services;
using Orchard.Mvc;
using Orchard.Security;
using Orchard.Themes;

namespace Cascade.WebShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly dynamic _shapeFactory;
        private readonly IOrderService _orderService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IShoppingCart _shoppingCart;
        private readonly ICustomerService _customerService;
        private readonly Localizer _t;
        private readonly IEnumerable<IPaymentServiceProvider> _paymentServiceProviders;
        private readonly IMessageManager _messageManager;
        private readonly IWebshopSettingsService _webshopSettings;

        public OrderController(IShapeFactory shapeFactory, IOrderService orderService, IAuthenticationService authenticationService,
            IShoppingCart shoppingCart, ICustomerService customerService, IEnumerable<IPaymentServiceProvider> paymentServiceProviders,
            IMessageManager messageManager, IWebshopSettingsService webshopSettings)
        {
            _shapeFactory = shapeFactory;
            _orderService = orderService;
            _authenticationService = authenticationService;
            _shoppingCart = shoppingCart;
            _customerService = customerService;
            _t = NullLocalizer.Instance;
            _paymentServiceProviders = paymentServiceProviders;
            _messageManager = messageManager;
            _webshopSettings = webshopSettings;
        }

        [Themed, HttpPost]
        public ActionResult Create()
        {
            var user = _authenticationService.GetAuthenticatedUser();

            if (user == null)
                throw new OrchardSecurityException(_t("Login required"));

            var customer = user.ContentItem.As<CustomerPart>();

            if (customer == null)
                throw new InvalidOperationException("The current user is not a customer");

            var order = _orderService.CreateOrder(customer.Id, _shoppingCart.Items);

            // Todo: Give paymet service providers a chance to process payment by sending a event. 
            // If no PSP handled the event, we'll just continue by displaying the created order.
            // Raise an OrderCreated event
            // Fire the PaymentRequest event
            var paymentRequest = new PaymentRequest(order);

            foreach (var handler in _paymentServiceProviders)
            {
                handler.RequestPayment(paymentRequest);

                // If the handler responded, it will set the action result
                if (paymentRequest.WillHandlePayment)
                {
                    return paymentRequest.ActionResult;
                }
            }

            // If we got here, no PSP handled the OrderCreated event, so we'll just display the order.
            var shape = _shapeFactory.Order_Created(
                Order: order,
                Products: _orderService.GetProducts(order.Details).ToArray(),
                Customer: customer,
                InvoiceAddress: (dynamic)_customerService.GetAddress(user.Id, "InvoiceAddress"),
                ShippingAddress: (dynamic)_customerService.GetAddress(user.Id, "ShippingAddress")
            );
            return new ShapeResult(this, shape);
        }

        [Themed]
        public ActionResult PaymentResponse()
        {

            var args = new PaymentResponse(HttpContext);

            foreach (var handler in _paymentServiceProviders)
            {
                handler.ProcessResponse(args);

                if (args.WillHandleResponse)
                    break;
            }

            if (!args.WillHandleResponse)
                throw new OrchardException(_t("Such things mean trouble"));

            var order = _orderService.GetOrderByNumber(args.OrderReference);
            _orderService.UpdateOrderStatus(order, args);

            var user = _authenticationService.GetAuthenticatedUser();

            if (user == null)
                throw new OrchardSecurityException(_t("Login required"));

            var customer = user.ContentItem.As<CustomerPart>();

            if (customer == null)
                throw new InvalidOperationException("The current user is not a customer");

            if (order.Status == OrderStatus.Paid)
            {
                // send an email confirmation to the customer
                string subject = string.Format("Your Order Number {0} has been received", order.Id);
                string body = string.Format("Dear {0}<br/><br/>Thank you for your order.<br/><br/>You will receive a Tax Invoice when your order is shipped.", customer.FirstName);
                _messageManager.Send(user.ContentItem.Record, "ORDER_RECEIVED", "email", new Dictionary<string, string>
                { 
                    { "Subject", subject }, 
                    { "Body", body} 
                });

                // send a copy of the order to the administator
                string adminEmail = _webshopSettings.GetAdministratorEmail();
                if (!string.IsNullOrWhiteSpace(adminEmail))
                    _messageManager.Send(new List<string> { adminEmail}, "ORDER_RECEIVED", "email", new Dictionary<string, string> 
                    { 
                        { "Subject", string.Format("Order Number {0} received from {1} {2}", order.Id, customer.FirstName, customer.LastName) }, 
                        { "Body", body} 
                    });

                // decrement stock levels
                _shoppingCart.RemoveFromStock();

                // finally, clear the order
                _shoppingCart.Clear();
            }

            return new ShapeResult(this, _shapeFactory.Order_PaymentResponse(Order: order, PaymentResponse: args, ContinueShoppingUrl: _webshopSettings.GetContinueShoppingUrl()));
        }


    }
}