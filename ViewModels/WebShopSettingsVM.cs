using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cascade.WebShop.Models;

namespace Cascade.WebShop.ViewModels
{
    public class WebShopSettingsVM
    {

        public IEnumerable<ShippingProductRecord> ShippingProducts { get; set; }

        [Required]
        public int? ShippingProductRecordId { get; set; }

        [Required]
        public string AdministratorEmailAddress
        { get; set; }

        [Required]
        public string ContinueShoppingUrl
        { get; set; }
        public bool ShowSubscribeToMailingList
        { get; set; }
        public bool SendWelcomeEmail
        { get; set; }
        public bool ShowTermsAndConditions
        { get; set; }
        public string TermsAndConditionsUrl
        { get; set; }
        public string PrivacyUrl
        { get; set; }
        public string WelcomeSubject
        { get; set; }
        public string WelcomeBodyTemplate
        { get; set; }
        public string UnsubscribeEmail
        { get; set; }
        public bool UseMailChimp
        { get; set; }
        public string MailChimpApiKey
        { get; set; }
        public string MailChimpListName
        { get; set; }
        public string MailChimpGroupName
        { get; set; }
        public string MailChimpGroupValue
        { get; set; }

    }
}

