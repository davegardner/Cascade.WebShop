using System;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement;
using Orchard.Security;
using Orchard.Users.Models;
using System.ComponentModel.DataAnnotations;

namespace Cascade.WebShop.Models
{
    public class CustomerRecord : ContentPartRecord
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Title { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
        public virtual bool SubscribeToMailingList { get; set; }
        public virtual bool AgreeTermsAndConditions { get; set; }
        public virtual bool WelcomeEmailPending { get; set; }
        public virtual bool ReceivePost { get; set; }

    }

    public class CustomerPart : ContentPart<CustomerRecord>
    {
        [Required]
        public string FirstName
        {
            get { return Retrieve(r=>r.FirstName); }
            set { Store(r=>r.FirstName, value); }
        }

        [Required]
        public string LastName
        {
            get { return Retrieve(r=>r.LastName); }
            set { Store(r=>r.LastName, value); }
        }

        public string Title
        {
            get { return Retrieve(r=>r.Title); }
            set { Store(r=>r.Title, value); }
        }

        public DateTime CreatedUtc
        {
            get { return Retrieve(r=>r.CreatedUtc); }
            set { Store(r=>r.CreatedUtc, value); }
        }

        public IUser User
        {
            get { return this.As<UserPart>(); }
        }

        public bool SubscribeToMailingList
        {
            get { return Retrieve(r=>r.SubscribeToMailingList); }
            set { Store(r=>r.SubscribeToMailingList, value); }
        }

        [Required]
        public bool AgreeTermsAndConditions
        {
            get { return Retrieve(r=>r.AgreeTermsAndConditions); }
            set { Store(r=>r.AgreeTermsAndConditions, value); }
        }
        public bool WelcomeEmailPending
        {
            get { return Retrieve(r=>r.WelcomeEmailPending); }
            set { Store(r=>r.WelcomeEmailPending, value); }
        }


        public bool ReceivePost
        {
            get { return Retrieve(r=>r.ReceivePost); }
            set { Store(r=>r.ReceivePost, value); }
        }

        public object FullName
        {
            get { return (FirstName + " " + LastName).Trim(); }
        }
    }
}