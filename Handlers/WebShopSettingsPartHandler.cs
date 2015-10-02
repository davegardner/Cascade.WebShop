using Cascade.WebShop.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization;

namespace Cascade.WebShop.Handlers
{
    public class WebShopSettingsPartHandler : ContentHandler {

        public Localizer T { get; set; }
        public WebShopSettingsPartHandler(IRepository<WebShopSettingsRecord> repository)
        {
            T = NullLocalizer.Instance;
            Filters.Add(new ActivatingFilter<WebShopSettingsPart>("Site"));
            Filters.Add(StorageFilter.For(repository));
            OnGetContentItemMetadata<WebShopSettingsPart>((context, part) => context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("WebShop"))));
        }
    }
}