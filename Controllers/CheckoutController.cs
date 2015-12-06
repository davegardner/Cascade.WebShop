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
        private readonly IMessageManager _messageManager;


        private Localizer T { get; set; }

        public CheckoutController(IOrchardServices services, IAuthenticationService authenticationService, ICustomerService customerService, IMembershipService membershipService,
            IShoppingCart shoppingCart, IWebshopSettingsService webshopSettings, IMessageManager messageManager)
        {
            _authenticationService = authenticationService;
            _services = services;
            _customerService = customerService;
            _membershipService = membershipService;
            _shoppingCart = shoppingCart;
            _webshopSettings = webshopSettings;
            _messageManager = messageManager;

            T = NullLocalizer.Instance;
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
            _messageManager.Send(new [] {signup.Email}, "WELCOME", "email", new Dictionary<string, string> 
                    { 
                        { "Subject", _webshopSettings.Settings.WelcomeSubject},
                        { "BodyTemplate", _webshopSettings.Settings.WelcomeBodyTemplate },
                        { "LastName", customer.LastName }, 
                        { "FirstName", customer.FirstName }, 
                        { "Subscribed", signup.ReceiveNewsletter.ToString() },
                        { "UnsubscribeEmail", _webshopSettings.Settings.UnsubscribeEmail}
                    });


            return RedirectToAction("SelectAddress");
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

            var invoiceAddress = _customerService.GetInvoiceAddress(customer.Id);
            var shippingAddress = _customerService.GetLastShippingAddress(customer.Id);

            var addressesViewModel = new AddressesViewModel
            {
                InvoiceAddress = MapAddress(invoiceAddress, customer),
                ShippingAddress = MapAddress(shippingAddress, customer)
            };

            var shape = _services.New.Checkout_SelectAddress(Addresses: addressesViewModel, ContinueShoppingUrl: _webshopSettings.GetContinueShoppingUrl());
            if (string.IsNullOrWhiteSpace(addressesViewModel.InvoiceAddress.Name))
                addressesViewModel.InvoiceAddress.Name = string.Format("{0} {1} {2}", customer.Title, customer.FirstName, customer.LastName);
            return new ShapeResult(this, shape);
        }

        [Themed, HttpPost]
        public ActionResult SelectAddress(AddressesViewModel addresses)
        {
            var currentUser = _authenticationService.GetAuthenticatedUser();

            if (currentUser == null)
                throw new OrchardSecurityException(T("Login required"));

            addresses.InvoiceAddress.CountryCodes = CountryCode.SelectList;
            addresses.ShippingAddress.CountryCodes = CountryCode.SelectList;

            if (!ModelState.IsValid)
            {
                return new ShapeResult(this, _services.New.Checkout_SelectAddress(Addresses: addresses, ContinueShoppingUrl: _webshopSettings.GetContinueShoppingUrl()));
            }

            var customer = currentUser.ContentItem.As<CustomerPart>();

            if (addresses.InvoiceAddress.IsValidAddress())
                MapAddress(addresses.InvoiceAddress, "InvoiceAddress", customer);
            if (addresses.ShippingAddress.IsValidAddress())
                MapAddress(addresses.ShippingAddress, "ShippingAddress", customer);

            // subscribe to mailing list if they asked for it
            if (_webshopSettings.Settings.UseMailChimp && customer.SubscribeToMailingList)
            {
                _messageManager.Send(new[] { currentUser.Email }, "SUBSCRIBE", "email", new Dictionary<string, string>
                    {
                        {"LastName", customer.LastName},
                        {"FirstName", customer.FirstName},
                        {"Address", addresses.InvoiceAddress.Address},
                        {"City", addresses.InvoiceAddress.City},
                        {"State", addresses.InvoiceAddress.State},
                        {"CountryCode", addresses.InvoiceAddress.CountryCode},
                        {"Postcode", addresses.InvoiceAddress.Postcode},
                        {"ReceivePost", customer.ReceivePost.ToString()} ,
                        {"MailChimpApiKey", _webshopSettings.Settings.MailChimpApiKey},
                        {"MailChimpListName", _webshopSettings.Settings.MailChimpListName},
                        {"MailChimpGroupName", _webshopSettings.Settings.MailChimpGroupName},
                        {"MailChimpGroupValue", _webshopSettings.Settings.MailChimpGroupValue}
                    });
            }

            return RedirectToAction("Summary");
        }

        private AddressViewModel MapAddress(AddressPart addressPart, CustomerPart customer)
        {
            var addressViewModel = new AddressViewModel();

            if (addressPart != null)
            {
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

        private AddressPart MapAddress(AddressViewModel source, string addressType, CustomerPart customerPart)
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
                addressPart = _customerService.CreateAddress(customerPart.Id, addressType, 0);
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
        public ActionResult Summary()
        {
            var user = _authenticationService.GetAuthenticatedUser();

            if (user == null)
                throw new OrchardSecurityException(T("Login required"));

            dynamic invoiceAddress = _customerService.GetInvoiceAddress(user.Id);
            dynamic shippingAddress = _customerService.GetLastShippingAddress(user.Id);
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

            return new ShapeResult(this, _services.New.Checkout_Summary(
                ShoppingCart: shoppingCartShape,
                InvoiceAddress: invoiceAddress,
                ShippingAddress: shippingAddress,
                ContinueShoppingUrl: _webshopSettings.GetContinueShoppingUrl()
            ));
        }

    }
}