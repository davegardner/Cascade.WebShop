using System.Linq;
using Cascade.WebShop.Models;
using Cascade.WebShop.ViewModels;
using Orchard;

namespace Cascade.WebShop.Services
{
    public interface IWebshopSettingsService : IDependency
    {
        string GetContinueShoppingUrl();
        string GetAdministratorEmail();
        bool GetShowSubscribeToMailingList();
        bool GetShowTermsAndConditions();
        string GetPrivacyUrl();
        int GetShippingProductRecordId();
        string GetTermsAndConditionsUrl();
        WebShopSettingsRecord Settings { get; }
        IQueryable<ShippingProductRecord> ShippingProductRecords();
        WebShopSettingsVM BuildWebShopVM(WebShopSettingsPart part);
        ShippingProductRecord GetShippingProduct(int id);
        void Map(WebShopSettingsPart part, WebShopSettingsVM vm);
    }
}