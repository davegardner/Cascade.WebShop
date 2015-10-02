using Orchard;
using Cascade.WebShop.Models;
using Orchard.ContentManagement;
using System.Collections.Generic;

namespace Cascade.WebShop.Services
{
    public interface ICustomerService : IDependency
    {
        CustomerPart CreateCustomer(string email, string password);
        AddressPart GetAddress(int customerId, string addressType);
        IEnumerable<AddressPart> GetAddresses(int id);
        AddressPart GetAddress(int id);
        AddressPart CreateAddress(int customerId, string addressType);
        IContentQuery<CustomerPart> GetCustomers(); 
        CustomerPart GetCustomer(int id);
        AddressPart GetShippingAddress(int id);

    }
}