using System.Collections.Generic;
using System.Web.Mvc;
using Cascade.WebShop.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.UI.Admin.Notification;
using Orchard.UI.Notify;

namespace Cascade.Paypal.Services
{
    public class MissingSettingsBanner : INotificationProvider
    {
        private readonly IOrchardServices _orchardServices;

        public MissingSettingsBanner(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public IEnumerable<NotifyEntry> GetNotifications()
        {
            var workContext = _orchardServices.WorkContext;
            var webshopSettingsPart = workContext.CurrentSite.As<WebShopSettingsPart>();

            if (webshopSettingsPart == null || !webshopSettingsPart.IsValid())
            {
                var urlHelper = new UrlHelper(workContext.HttpContext.Request.RequestContext);
                var url = urlHelper.Action("WebShop", "Admin", new { Area = "Settings" });
                yield return new NotifyEntry { Message = T("The <a href=\"{0}\">WebShop settings</a> need to be configured.", url), Type = NotifyType.Warning };
            }
        }
    }
}
