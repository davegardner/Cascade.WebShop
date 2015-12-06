using System.Linq;
using System.Web;
using Cascade.WebShop.Models;
using Orchard.Data;
using Cascade.WebShop.ViewModels;
using Orchard;

namespace Cascade.WebShop.Services
{
    // TODO Fix.
    // This class is very wrong. Should be using the technique under "Using site scope settings" 
    // here: http://www.szmyd.com.pl/blog/how-to-add-settings-to-your-content-parts-and-items#.UUq4tRf-HQU

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

    public class WebshopSettingsService : IWebshopSettingsService
    {
        private readonly IRepository<WebShopSettingsRecord> _repository;
        private readonly IRepository<ShippingProductRecord> _shippingProductRepository;
        private WebShopSettingsRecord _settings;

        public WebshopSettingsService(IRepository<WebShopSettingsRecord> repository, IRepository<ShippingProductRecord> shippingProductRepository)
        {
            _repository = repository;
            _shippingProductRepository = shippingProductRepository;
        }

        public WebShopSettingsRecord Settings
        {
            get { return _settings ?? (_settings = _repository.Table.FirstOrDefault()); }
        }

        /// <summary>
        /// Gets the absolute url of the Continue Shopping setting
        /// </summary>
        /// <returns></returns>
        public string GetContinueShoppingUrl()
        {
            return Settings == null ? "/" : PrettyPath(Settings.ContinueShoppingUrl);
        }

        /// <summary>
        /// Gets the admin email address
        /// </summary>
        /// <returns></returns>
        public string GetAdministratorEmail()
        {
            return Settings == null ? string.Empty : Settings.AdministratorEmailAddress;
        }

        public bool GetShowSubscribeToMailingList()
        {
            return Settings != null && Settings.ShowSubscribeToMailingList;
        }

        public bool GetShowTermsAndConditions()
        {
            return Settings != null && Settings.ShowTermsAndConditions;
        }

        public string GetTermsAndConditionsUrl()
        {
            return Settings == null ? "/" : PrettyPath(Settings.TermsAndConditionsUrl);
        }

        public string GetPrivacyUrl()
        {
            return Settings == null ? "/" : PrettyPath(Settings.PrivacyUrl);
        }

        public int GetShippingProductRecordId()
        {
            return Settings == null ? 0 : (Settings.ShippingProductRecord == null ? 0 : Settings.ShippingProductRecord.Id);
        }

        private string PrettyPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "/";

            return HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/" + path.Trim('/');
        }

        public IQueryable<ShippingProductRecord> ShippingProductRecords()
        {
            return _shippingProductRepository.Table.OrderBy(s => s.Title);
        }

        public WebShopSettingsVM BuildWebShopVM(WebShopSettingsPart part)
        {
            return new WebShopSettingsVM
            {
                ShippingProducts = ShippingProductRecords(),
                ShippingProductRecordId = part.ShippingProductRecord == null ? 0 : part.ShippingProductRecord.Id,
                AdministratorEmailAddress = part.AdministratorEmailAddress,
                ContinueShoppingUrl = part.ContinueShoppingUrl,
                ShowSubscribeToMailingList = part.ShowSubscribeToMailingList,
                SendWelcomeEmail = part.SendWelcomeEmail,
                ShowTermsAndConditions = part.ShowTermsAndConditions,
                TermsAndConditionsUrl = part.TermsAndConditionsUrl,
                PrivacyUrl = part.PrivacyUrl,
                WelcomeBodyTemplate = part.WelcomeBodyTemplate,
                WelcomeSubject = part.WelcomeSubject,
                UnsubscribeEmail = part.UnsubscribeEmail,
                UseMailChimp = part.UseMailChimp,
                MailChimpApiKey = part.MailChimpApiKey,
                MailChimpListName = part.MailChimpListName,
                MailChimpGroupName = part.MailChimpGroupName,
                MailChimpGroupValue = part.MailChimpGroupValue
            };
        }

        public ShippingProductRecord GetShippingProduct(int id)
        {
            return _shippingProductRepository.Get(id);
        }

        public void Map(WebShopSettingsPart part, WebShopSettingsVM vm)
        {
            if(vm.ShippingProductRecordId.HasValue)
                part.ShippingProductRecord = GetShippingProduct(vm.ShippingProductRecordId.Value);
            part.AdministratorEmailAddress = vm.AdministratorEmailAddress;
            part.ContinueShoppingUrl = vm.ContinueShoppingUrl;
            part.ShowSubscribeToMailingList = vm.ShowSubscribeToMailingList;
            part.SendWelcomeEmail = vm.SendWelcomeEmail;
            part.ShowTermsAndConditions = vm.ShowTermsAndConditions;
            part.TermsAndConditionsUrl = vm.TermsAndConditionsUrl;
            part.PrivacyUrl = vm.PrivacyUrl;
            part.WelcomeBodyTemplate = vm.WelcomeBodyTemplate;
            part.WelcomeSubject = vm.WelcomeSubject;
            part.UnsubscribeEmail = vm.UnsubscribeEmail;
            part.UseMailChimp = vm.UseMailChimp;
            part.MailChimpApiKey = vm.MailChimpApiKey;
            part.MailChimpListName = vm.MailChimpListName;
            part.MailChimpGroupName = vm.MailChimpGroupName;
            part.MailChimpGroupValue = vm.MailChimpGroupValue;
        }
    }
}