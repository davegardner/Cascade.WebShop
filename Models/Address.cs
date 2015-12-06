using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Cascade.WebShop.Models
{
    public class AddressRecord : ContentPartRecord
    {

        public virtual int CustomerId { get; set; }
        public virtual int OrderId { get; set; }
        public virtual string Type { get; set; }
        public virtual string Name { get; set; }
        public virtual string Address { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Postcode { get; set; }
        public virtual string CountryCode { get; set; }
    }
    
    public class AddressPart : ContentPart<AddressRecord>
    {

        public int CustomerId
        {
            get { return Retrieve(r=>r.CustomerId); }
            set { Store(r=>r.CustomerId, value); }
        }

        public int OrderId
        {
            get { return Retrieve(r => r.OrderId); }
            set { Store(r => r.OrderId, value); }
        }

        public string Type
        {
            get { return Retrieve(r=>r.Type); }
            set { Store(r=>r.Type, value); }
        }

        public string Name
        {
            get { return Retrieve(r=>r.Name); }
            set { Store(r=>r.Name, value); }
        }

        public string Address
        {
            get { return Retrieve(r=>r.Address); }
            set { Store(r=>r.Address, value); }
        }

        public string City
        {
            get { return Retrieve(r=>r.City); }
            set { Record.City= value; }
        }

        public string State
        {
            get { return Retrieve(r=>r.State); }
            set { Store(r=>r.State, value); }
        }

        public string Postcode
        {
            get { return Retrieve(r=>r.Postcode); }
            set { Store(r=>r.Postcode, value); }
        }

        public string CountryCode
        {
            get { return Retrieve(r=>r.CountryCode); }
            set { Store(r=>r.CountryCode, value); }
        }
        public string Country
        {
            get { return Cascade.WebShop.Helpers.CountryCode.GetCountry(Record.CountryCode); }
        }
    }
}