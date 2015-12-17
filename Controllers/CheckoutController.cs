using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Cascade.WebShop.Helpers;
using Cascade.WebShop.Models;
using Cascade.WebShop.Services;
using Cascade.WebShop.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Messaging.Services;
using Orchard.Mvc;
using Orchard.Security;
using Orchard.Themes;
using Orchard.Email.Services;
using System;
using Cascade.WebShop.Settings;

namespace Cascade.WebShop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IOrchardServices _services;
        private readonly ICustomerService _customerService;
        private readonly IMembershipService _membershipService;
        private readonly IShoppingCart _shoppingCart;
        private readonly IWebshopSettingsService _webshopSettings;
        private readonly IOrderService _orderService;
        private readonly ISmtpChannel _email;


        private Localizer T { get; set; }

        public CheckoutController(IOrchardServices services,
            IAuthenticationService authenticationService,
            ICustomerService customerService,
            IMembershipService membershipService,
            IShoppingCart shoppingCart,
            IWebshopSettingsService webshopSettings,
            IOrderService orderService,
            ISmtpChannel email)
        {
            _authenticationService = authenticationService;
            _services = services;
            _customerService = customerService;
            _membershipService = membershipService;
            _shoppingCart = shoppingCart;
            _webshopSettings = webshopSettings;
            _email = email;
            T = NullLocalizer.Instance;
            _orderService = orderService;
        }

        [Themed]
        public ActionResult SignupOrLogin()
        {
            var user = _authenticationService.GetAuthenticatedUser();

            // don't allow users who are not customers
            if (user == null || user.ContentItem.As<CustomerPart>() == null)
                return new ShapeResult(this, _services.New.Checkout_SignupOrLogin());

            return RedirectToAction("SelectAddress");
        }

        [Themed]
        public ActionResult Signup()
        {
            var shape = _services.New.Checkout_Signup(Signup: new SignupViewModel
            {
                ShowReceiveNewsletter = _webshopSettings.GetShowSubscribeToMailingList(),
                ShowAcceptTerms = _webshopSettings.GetShowTermsAndConditions(),
                TermsAndConditionsUrl = _webshopSettings.GetTermsAndConditionsUrl(),
                PrivacyUrl = _webshopSettings.GetPrivacyUrl(),
                ContinueShoppingUrl = _webshopSettings.GetContinueShoppingUrl()
            });
            return new ShapeResult(this, shape);
        }


        [Themed, HttpPost]
        public ActionResult Signup(SignupViewModel signup)
        {
            if (!ModelState.IsValid)
            {
                return new ShapeResult(this, _services.New.Checkout_Signup(Signup: signup));
            }

            var customer = _customerService.CreateCustomer(signup.Email, signup.Password);
            customer.FirstName = signup.FirstName;
            customer.LastName = signup.LastName;
            customer.Title = signup.Title;
            customer.AgreeTermsAndConditions = signup.AcceptTerms;
            customer.SubscribeToMailingList = signup.ReceiveNewsletter;
            customer.ReceivePost = signup.ReceivePost;

            _authenticationService.SignIn(customer.User, true);

            // Welcome message gets sent whether or not they subscribe to the mailing list
            _email.Process(new Dictionary<string, object>
            {
                { "Recipients", signup.Email},
                { "Subject", _webshopSettings.Settings.WelcomeSubject},
                { "BodyTemplate", MergeBody(customer) },
            });

            return RedirectToAction("SelectAddress");
        }

        private object MergeBody(CustomerPart customer)
        {
            string body = String.Empty;
            string unsubscribe = _webshopSettings.Settings.UnsubscribeEmail;
            if (!String.IsNullOrWhiteSpace(_webshopSettings.Settings.WelcomeBodyTemplate))
            {
                body = string.Format(_webshopSettings.Settings.WelcomeBodyTemplate,
                    customer.FirstName, customer.LastName);
            }
            if (customer.SubscribeToMailingList && !string.IsNullOrWhiteSpace(unsubscribe))
                body += "<br/><br/>PS You are subscribed to our mailing list. To unsubscribe please send an email to " + unsubscribe;
            return body;
        }


        [Themed]
        public ActionResult Login()
        {
            var shape = _services.New.Checkout_Login();
            return new ShapeResult(this, shape);
        }

        [Themed, HttpPost]
        public ActionResult Login(LoginViewModel login)
        {

            // Validate the specified credentials
            var user = _membershipService.ValidateUser(login.Email, login.Password);

            // If no user was found, add a model error
            if (user == null)
            {
                ModelState.AddModelError("Email", T("Unknown username/password").ToString());
            }
            else
                // Don't allow users who are not customers
                if (user.ContentItem.As<CustomerPart>() == null)
                    ModelState.AddModelError("UserType", "Valid User but not a Customer. Please register as a Customer first.");

            // If there are any model errors, redisplay the login form
            if (!ModelState.IsValid)
            {
                var shape = _services.New.Checkout_Login(Login: login);
                return new ShapeResult(this, shape);
            }

            // Create a forms ticket for the user
            _authenticationService.SignIn(user, login.CreatePersistentCookie);

            // Redirect to the next step
            return RedirectToAction("SelectAddress");
        }

        [Themed]
        public ActionResult SelectAddress()
        {
            var currentUser = _authenticationService.GetAuthenticatedUser();

            if (currentUser == null)
                throw new OrchardSecurityException(T("Login required"));

            var customer = currentUser.ContentItem.As<CustomerPart>();

            if (customer == null)
            {
                //throw new Exception("Logged on user is NOT a customer.");
                return RedirectToAction("SignupOrLogin");
            }

            //var orderIdObject = HttpContext.Session["orderid"];
            //var orderId = (int)orderIdObject;

            var invoiceAddress = _customerService.GetInvoiceAddress(customer.Id);
            var shippingAddress = _customerService.GetLastShippingAddress(customer.Id);

            var addressesViewModel = new AddressesViewModel
            {
                InvoiceAddress = MapAddress(invoiceAddress, customer),
                ShippingAddress = MapAddress(shippingAddress, customer),
                ShippingAddressSupplied = false
            };

            dynamic shape;
            // if any product is a Booking then use the Booking Template
            if (_shoppingCart.GetProductParts().Any(p => p.Settings.GetModel<ProductSettings>().Mode == ProductMode.Booking))
            {
                shape = _services.New.Checkout_Booking_SelectAddress(
                    Addresses: addressesViewModel,
                    ContinueShoppingUrl: _webshopSettings.GetContinueShoppingUrl(),
                    Booking: new BookingVM()
                    );
            }
            else
            {
                shape = _services.New.Checkout_Product_SelectAddress(
                    Addresses: addressesViewModel,
                    ContinueShoppingUrl: _webshopSettings.GetContinueShoppingUrl()
                    );
            }

            if (string.IsNullOrWhiteSpace(addressesViewModel.InvoiceAddress.Name))
                addressesViewModel.InvoiceAddress.Name = string.Format("{0} {1} {2}", customer.Title, customer.FirstName, customer.LastName);

            return new ShapeResult(this, shape);
        }

        [Themed, HttpPost]
        public ActionResult SelectAddress(AddressesViewModel addresses, BookingVM booking)
        {
            var currentUser = _authenticationService.GetAuthenticatedUser();

            if (currentUser == null)
                throw new OrchardSecurityException(T("Login required"));

            var isBooking = _shoppingCart.GetProductParts()
                .Any(p => p.Settings.GetModel<ProductSettings>().Mode == ProductMode.Booking);

            if (!addresses.InvoiceAddress.IsValidAddress())
                ModelState.AddModelError("InvalidInvoiceAddress", "Please correct the Invoice Address and try again.");

            if (addresses.ShippingAddressSupplied && !addresses.ShippingAddress.IsValidAddress())
            {
                if (isBooking)
                    ModelState.AddModelError("InvalidBookingAddress", "Please correct the Booking Address and try again.");
                else
                    ModelState.AddModelError("InvalidShippingAddress", "Please correct the Shipping Address and try again.");
            }

            if (!ModelState.IsValid)
            {
                addresses.InvoiceAddress.CountryCodes = CountryCode.SelectList;
                addresses.ShippingAddress.CountryCodes = CountryCode.SelectList;

                if (isBooking)
                {
                    return new ShapeResult(this, _services.New.Checkout_Booking_SelectAddress(
                        Addresses: addresses,
                        ContinueShoppingUrl: _webshopSettings.GetContinueShoppingUrl(),
                        Booking : booking
                        ));
                }
                else
                {
                    return new ShapeResult(this, _services.New.Checkout_Product_SelectAddress(
                        Addresses: addresses,
                        ContinueShoppingUrl: _webshopSettings.GetContinueShoppingUrl()
                        ));
                }
            }

            var customer = currentUser.ContentItem.As<CustomerPart>();
            var order = _orderService.CreateOrder(customer.Id, _shoppingCart.Items, booking);

            // save order id for Review action and order controller
            HttpContext.Session["orderid"] = order.Id;

            MapAddress(addresses.InvoiceAddress, "InvoiceAddress", customer);

            if (addresses.ShippingAddressSupplied)
                MapAddress(addresses.ShippingAddress, "ShippingAddress", customer, order);


            return RedirectToAction("Review");
        }

        private AddressViewModel MapAddress(AddressPart addressPart, CustomerPart customer)
        {
            var addressViewModel = new AddressViewModel();

            if (addressPart != null)
            {
                addressViewModel.AddressType = addressPart.Type;
                addressViewModel.Name = addressPart.Name;
                addressViewModel.Address = addressPart.Address;
                addressViewModel.State = addressPart.State;
                addressViewModel.Postcode = addressPart.Postcode;
                addressViewModel.City = addressPart.City;
                addressViewModel.CountryCode = addressPart.CountryCode;
            }

            addressViewModel.CountryCodes = CountryCode.SelectList;

            return addressViewModel;
        }

        private AddressPart MapAddress(AddressViewModel source, string addressType, CustomerPart customerPart, OrderPart order = null)
        {
            // Allow for many different Shipping Addresses: one for each order
            AddressPart addressPart;
            if (addressType == "InvoiceAddress")
            {
                addressPart = _customerService.GetInvoiceAddress(customerPart.Id);
                if (addressPart == null)
                    addressPart = _customerService.CreateAddress(customerPart.Id, addressType, 0);
            }
            else
            {
                addressPart = _customerService.CreateAddress(customerPart.Id, addressType, order == null ? 0 : order.Id);
            }

            addressPart.Name = source.Name.TrimSafe();
            addressPart.Address = source.Address.TrimSafe();
            addressPart.State = source.State.TrimSafe();
            addressPart.Postcode = source.Postcode.TrimSafe();
            addressPart.City = source.City.TrimSafe();
            addressPart.CountryCode = source.CountryCode.TrimSafe();

            return addressPart;
        }

        [Themed]
        public ActionResult Review()
        {
            var user = _authenticationService.GetAuthenticatedUser();

            if (user == null)
                throw new OrchardSecurityException(T("Login required"));

            dynamic shippingAddress = null;
            var orderIdObject = HttpContext.Session["orderid"];
            var orderId = (int)orderIdObject;
            shippingAddress = _customerService.GetShippingAddress(user.Id, orderId);

            dynamic invoiceAddress = _customerService.GetInvoiceAddress(user.Id);
            dynamic shoppingCartShape = _services.New.ShoppingCart();

            var query = _shoppingCart.GetProducts().Select(x => _services.New.ShoppingCartItem(
                Product: x.ProductPart,
                Quantity: x.Quantity,
                Title: _services.ContentManager.GetItemMetadata(x.ProductPart).DisplayText
            ));

            shoppingCartShape.ShopItems = query.ToArray();
            shoppingCartShape.Total = _shoppingCart.Total();
            shoppingCartShape.Subtotal = _shoppingCart.Subtotal();
            shoppingCartShape.GST = _shoppingCart.GST();

            var order = _orderService.GetOrder(orderId);
            dynamic shape;

            // Select the template depending on whether any product is a Booking
            if (_orderService.ProductWithMode(order, ProductMode.Booking))

                shape = new ShapeResult(this, _services.New.Checkout_Booking_Review(
                    ShoppingCart: shoppingCartShape,
                    InvoiceAddress: invoiceAddress,
                    ShippingAddress: shippingAddress,
                    ContinueShoppingUrl: _webshopSettings.GetContinueShoppingUrl(),
                    Order: order
                ));
            else
                shape = new ShapeResult(this, _services.New.Checkout_Product_Review(
                    ShoppingCart: shoppingCartShape,
                    InvoiceAddress: invoiceAddress,
                    ShippingAddress: shippingAddress,
                    ContinueShoppingUrl: _webshopSettings.GetContinueShoppingUrl(),
                    Order: order
                ));
            return shape;
        }

    }
}