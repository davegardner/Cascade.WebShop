using Orchard.ContentManagement.Utilities;
using Orchard.Data;
using Cascade.WebShop.Models;

namespace Cascade.WebShop.Handlers
{
    public class AddressPartHandler : ContentHandler
    {
        public AddressPartHandler(IRepository<AddressRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}