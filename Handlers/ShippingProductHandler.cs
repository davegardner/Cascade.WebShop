using Cascade.WebShop.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Cascade.WebShop.Handlers
{
    public class ShippingProductHandler : ContentHandler
    {
        public ShippingProductHandler(IRepository<ShippingProductRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}