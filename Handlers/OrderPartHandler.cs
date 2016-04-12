using Orchard.ContentManagement.Utilities;
using Orchard.Data;
using Cascade.WebShop.Models;
using Orchard.ContentManagement;
using System.Linq;
using Cascade.WebShop.Helpers;

namespace Cascade.WebShop.Handlers
{
    public class OrderPartHandler : ContentHandler
    {
        public OrderPartHandler(IRepository<OrderRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
            OnLoaded<OrderPart>((context, part) =>
            {
                if (part == null)
                    return;

                part.Details = OrderDetailSerializer.Deserialize(part.RawDetails);

            });

            OnUpdated<OrderPart>((context, part) =>
            {
                if (part == null)
                    return;

                part.RawDetails = OrderDetailSerializer.Serialize(part.Details);
            });
        }
    }
}