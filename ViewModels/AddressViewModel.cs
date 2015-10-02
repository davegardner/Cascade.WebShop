using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Cascade.WebShop.ViewModels
{
    public class AddressViewModel
    {
        public AddressViewModel()
        {
            CountryCode = "AU";
        }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(256)]
        public string Address { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(256)]
        public string State { get; set; }

        [StringLength(10)]
        public string Postcode { get; set; }

        [StringLength(2)]
        public string CountryCode { get; set; }

        public IEnumerable<SelectListItem> CountryCodes { get; set; }

        public bool IsValidAddress()
        {
            return !(string.IsNullOrWhiteSpace(City)
                || string.IsNullOrWhiteSpace(Postcode)
                || string.IsNullOrWhiteSpace(Address));
        }


    }

}