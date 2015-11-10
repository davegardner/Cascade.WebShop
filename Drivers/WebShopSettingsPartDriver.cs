using Cascade.WebShop.Models;
using Cascade.WebShop.Services;
using Cascade.WebShop.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Settings;

namespace Cascade.WebShop.Drivers
{
    public class WebShopSettingsPartDriver : ContentPartDriver<WebShopSettingsPart>
    {
        //private readonly ISiteService _siteService;
        private readonly IWebshopSettingsService _webshopSettings;
        protected override string Prefix { get { return "WebShopSettings"; } }

        private const string shapeName = "Parts_WebShop_Settings_Edit";       // "Parts_Settings_WebShopSettings"
        private const string templateName = "Parts/WebShop.Settings";    // "Parts.Settings.WebShopSettings"
        
        public WebShopSettingsPartDriver(IWebshopSettingsService webshopSettings, ISiteService siteService)
        {
            _webshopSettings = webshopSettings;
            //_siteService = siteService;
        }

        protected override DriverResult Editor(WebShopSettingsPart part, dynamic shapeHelper)
        {
            //var settings = _siteService.GetSiteSettings().As<WebShopSettingsPart>();
            //var model = new WebShopSettingsVM
            //{
            //    _part = settings,
            //    _shippingProducts = _webshopSettings.ShippingProductRecords()
            //};

            var model = _webshopSettings.BuildWebShopVM(part);

            return ContentShape(shapeName,
                () => shapeHelper.EditorTemplate(TemplateName: templateName, Model: model, Prefix: Prefix)).OnGroup("WebShop");
        }

        protected override DriverResult Editor(WebShopSettingsPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            //var settings = _siteService.GetSiteSettings().As<WebShopSettingsPart>();
            //var model = new WebShopSettingsVM
            //{
            //    _part = settings,
            //    _shippingProducts = _webshopSettings.ShippingProductRecords()
            //};

            var model = _webshopSettings.BuildWebShopVM(part);

            if (updater.TryUpdateModel(model, Prefix, null, null))
            {
                _webshopSettings.Map(part, model);
            }

            return ContentShape(shapeName,
                () => shapeHelper.EditorTemplate(TemplateName: templateName, Model: model, Prefix: Prefix)).OnGroup("WebShop");
        }

    }
}