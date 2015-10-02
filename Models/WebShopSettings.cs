using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using System.ComponentModel.DataAnnotations;

namespace Cascade.WebShop.Models
{
    public class WebShopSettingsRecord : ContentPartRecord
    {
        public virtual string AdministratorEmailAddress { get; set; }
        public virtual string ContinueShoppingUrl { get; set; }
        public virtual string TermsAndConditionsUrl { get; set; }
        public virtual string PrivacyUrl { get; set; }
        public virtual bool ShowSubscribeToMailingList { get; set; }
        public virtual bool SendWelcomeEmail{ get; set; }
        public virtual bool ShowTermsAndConditions { get; set; }
        //public virtual bool AgreeTermsAndConditions { get; set; }
        public virtual string WelcomeSubject { get; set; }
        public virtual string WelcomeBodyTemplate { get; set; }
        public virtual string UnsubscribeEmail{ get; set; }
        public virtual bool UseMailChimp { get; set; }
        public virtual string MailChimpApiKey { get; set; }
        public virtual string MailChimpListName { get; set; }
        public virtual string MailChimpGroupName { get; set; }
        public virtual string MailChimpGroupValue { get; set; }
        public virtual ShippingProductRecord ShippingProductRecord { get; set; }
    }

    public class WebShopSettingsPart : ContentPart<WebShopSettingsRecord>
    {
        [Required]
        public string AdministratorEmailAddress
        {
            get { return Retrieve(r=>r.AdministratorEmailAddress); }
            set { Store(r=>r.AdministratorEmailAddress, value); }
        }
        [Required]
        public string ContinueShoppingUrl
        {
            get { return Retrieve(r=>r.ContinueShoppingUrl); }
            set { Store(r=>r.ContinueShoppingUrl, value); }
        }
        public bool ShowSubscribeToMailingList
        {
            get { return Retrieve(r=>r.ShowSubscribeToMailingList); }
            set { Store(r=>r.ShowSubscribeToMailingList, value); }
        }
        public bool SendWelcomeEmail
        {
            get { return Retrieve(r=>r.SendWelcomeEmail); }
            set { Store(r=>r.SendWelcomeEmail, value); }
        }
        public bool ShowTermsAndConditions
        {
            get { return Retrieve(r=>r.ShowTermsAndConditions); }
            set { Store(r=>r.ShowTermsAndConditions, value); }
        }
        public string TermsAndConditionsUrl 
        {
            get { return Retrieve(r=>r.TermsAndConditionsUrl); } 
            set{Store(r=>r.TermsAndConditionsUrl, value);}
        }
        public string PrivacyUrl 
        {
            get { return Retrieve(r=>r.PrivacyUrl); } 
            set{Store(r=>r.PrivacyUrl, value);}
       }
        public string WelcomeSubject
        {
            get { return Retrieve(r=>r.WelcomeSubject); }
            set { Store(r=>r.WelcomeSubject, value); }
        }
        public string WelcomeBodyTemplate
        {
            get { return Retrieve(r=>r.WelcomeBodyTemplate); }
            set { Store(r=>r.WelcomeBodyTemplate, value); }
        }
        public string UnsubscribeEmail
        {
            get { return Retrieve(r=>r.UnsubscribeEmail); }
            set { Store(r=>r.UnsubscribeEmail, value); }
        }
        public bool UseMailChimp
        {
            get { return Retrieve(r=>r.UseMailChimp); }
            set { Store(r=>r.UseMailChimp, value); }
        }
        public string MailChimpApiKey
        {
            get { return Retrieve(r=>r.MailChimpApiKey); }
            set { Store(r=>r.MailChimpApiKey, value); }
        }

        public string MailChimpListName
        {
            get { return Retrieve(r=>r.MailChimpListName); }
            set { Store(r=>r.MailChimpListName, value); }
        }
        public string MailChimpGroupName
        {
            get { return Retrieve(r=>r.MailChimpGroupName); }
            set { Store(r=>r.MailChimpGroupName, value); }
        }
        public string MailChimpGroupValue
        {
            get { return Retrieve(r=>r.MailChimpGroupValue); }
            set { Store(r=>r.MailChimpGroupValue, value); }
        }

        public bool IsValid()
        {
            return !(Record == null)
                && !string.IsNullOrWhiteSpace(Record.AdministratorEmailAddress)
                && !string.IsNullOrWhiteSpace(Record.ContinueShoppingUrl);
        }

        public ShippingProductRecord ShippingProductRecord
        {
            get { return Record.ShippingProductRecord; }
            set { Record.ShippingProductRecord = value; }
        }
    }
}