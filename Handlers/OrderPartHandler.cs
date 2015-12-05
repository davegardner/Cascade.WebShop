using Orchard.ContentManagement.Handlers;
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
            OnLoaded<OrderRecordPart>((context, part) =>
            {
                if (part == null)
                    return;

                // Deserialize Elements
                part.Details = OrderDetailSerializer.Deserialize(part.RawDetails);

            });

            OnUpdated<OrderRecordPart>((context, part) =>
            {
                if (part == null)
                    return;

                // Serialize Elements
                part.RawDetails = OrderDetailSerializer.Serialize(part.Details);
            });
        }
    }
}