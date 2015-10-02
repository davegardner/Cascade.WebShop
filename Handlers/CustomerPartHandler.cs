using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Users.Models;
using Cascade.WebShop.Models;

namespace Cascade.WebShop.Handlers
{
    public class CustomerPartHandler : ContentHandler
    {
        public CustomerPartHandler(IRepository<CustomerRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
            Filters.Add(new ActivatingFilter<UserPart>("Customer"));
        }
    }
}