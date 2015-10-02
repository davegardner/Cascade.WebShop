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
    public class CustomerService : ICustomerService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IMembershipService _membershipService;
        private readonly IClock _clock;

        public CustomerService(IOrchardServices orchardServices, IMembershipService membershipService, IClock clock)
        {
            _orchardServices = orchardServices;
            _membershipService = membershipService;
            _clock = clock;
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

        public AddressPart GetAddress(int customerId, string addressType)
        {
            return _orchardServices.ContentManager.Query<AddressPart, AddressRecord>().Where(x => x.CustomerId == customerId && x.Type == addressType).List().FirstOrDefault();
        }

        public AddressPart GetAddress(int id)
        {
            var address = _orchardServices.ContentManager.Get<AddressPart>(id);
            return address;
        }


        public IEnumerable<AddressPart> GetAddresses(int id)
        {
            return _orchardServices.ContentManager.Query<AddressPart, AddressRecord>().Where(x=>x.CustomerId == id).List();
        }
        
        public AddressPart CreateAddress(int customerId, string addressType)
        {
            return _orchardServices.ContentManager.Create<AddressPart>("Address", x =>
            {
                x.Type = addressType;
                x.CustomerId = customerId;
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

        public AddressPart GetShippingAddress(int id)
        {
            var addresses = GetAddresses(id);
            if (addresses != null && addresses.Count() > 0)
            {
                // return shipping address with largest id (ie: most recent)
                return addresses.OrderByDescending(a => a.Type + a.Id.ToString("000000")).First();
            }
         
            return null;
        }

    }
}