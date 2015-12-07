using Orchard;
using Orchard.ContentManagement;
using Orchard.Security;
using Orchard.Services;
using Orchard.Users.Models;
using Cascade.WebShop.Models;
using System.Linq;
using System.Collections.Generic;

namespace Cascade.WebShop.Services
{
    public interface ICustomerService : IDependency
    {
        CustomerPart CreateCustomer(string email, string password);
        //AddressPart GetAddress(int customerId, string addressType);
        IEnumerable<AddressPart> GetAddresses(int id);
        AddressPart GetAddress(int id);
        AddressPart CreateAddress(int customerId, string addressType, int orderId);
        IContentQuery<CustomerPart> GetCustomers();
        CustomerPart GetCustomer(int id);
        /// <summary>
        /// Retrieve the most recent Invoice address for this customer
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <returns>AddressPart</returns>
        AddressPart GetInvoiceAddress(int customerId);
        /// <summary>
        /// Get the only shipping address for the specified customer and order
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <param name="orderId">Order Id</param>
        /// <returns>AddressPart</returns>
        AddressPart GetShippingAddress(int customerId, int orderId);
        /// <summary>
        /// Retrieves the most recently used shipping address for this customer, as a decent default value
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        AddressPart GetLastShippingAddress(int customerId);

    }

    public class CustomerService : ICustomerService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IMembershipService _membershipService;
        private readonly IClock _clock;
        private readonly IOrderService _orderService;

        public CustomerService(IOrchardServices orchardServices, IMembershipService membershipService, IClock clock, IOrderService orderService)
        {
            _orchardServices = orchardServices;
            _membershipService = membershipService;
            _clock = clock;
            _orderService = orderService;
        }

        public CustomerPart CreateCustomer(string email, string password)
        {
            // New up a new content item of type "Customer"
            var customer = _orchardServices.ContentManager.New("Customer");

            // Cast the customer to a UserPart
            var userPart = customer.As<UserPart>();

            // Cast the customer to a CustomerPart
            var customerPart = customer.As<CustomerPart>();

            // Set some properties of the customer content item (via UserPart and CustomerPart)
            userPart.UserName = email;
            userPart.Email = email;
            userPart.NormalizedUserName = email.ToLowerInvariant();
            userPart.Record.HashAlgorithm = "SHA1";
            userPart.Record.RegistrationStatus = UserStatus.Approved;
            userPart.Record.EmailStatus = UserStatus.Approved;

            // Use IClock to get the current date instead of using DateTime.Now (see http://skywalkersoftwaredevelopment.net/orchard-development/api/iclock)
            customerPart.CreatedUtc = _clock.UtcNow;

            // Use Ochard's MembershipService to set the password of our new user
            _membershipService.SetPassword(userPart, password);

            // Store the new user into the database
            _orchardServices.ContentManager.Create(customer);

            return customerPart;
        }

        public AddressPart GetAddress(int id)
        {
            var address = _orchardServices.ContentManager.Get<AddressPart>(id);
            return address;
        }


        public IEnumerable<AddressPart> GetAddresses(int customerId)
        {
            return _orchardServices.ContentManager.Query<AddressPart, AddressRecord>()
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(a => a.Id)
                .List();
        }

        public AddressPart CreateAddress(int customerId, string addressType, int orderId)
        {
            return _orchardServices.ContentManager.Create<AddressPart>("Address", x =>
            {
                x.Type = addressType;
                x.CustomerId = customerId;
                x.OrderId = orderId;
            });
        }

        public CustomerPart GetCustomer(int id)
        {
            return _orchardServices.ContentManager.Get<CustomerPart>(id);
        }

        public IContentQuery<CustomerPart> GetCustomers()
        {
            return _orchardServices.ContentManager.Query<CustomerPart, CustomerRecord>();
        }

        public AddressPart GetShippingAddress(int customerId, int orderId)
        {
            return GetAddresses(customerId).FirstOrDefault(a => a.OrderId == orderId);
        }

        public AddressPart GetInvoiceAddress(int customerId)
        {
            return GetAddresses(customerId).FirstOrDefault(a => a.Type == "InvoiceAddress");
        }

        public AddressPart GetLastShippingAddress(int customerId)
        {
            var lastOrder = _orderService.GetOrders(customerId)
                .OrderByDescending<OrderRecord>(o=>o.Id)
                .Slice(1)
                .FirstOrDefault();

            if(lastOrder == null)
                return null;

            return GetShippingAddress(customerId, lastOrder.Id);
        }
    }
}