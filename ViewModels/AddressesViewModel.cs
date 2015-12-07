using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cascade.WebShop.ViewModels
{
    public class AddressesViewModel : IValidatableObject
    {

        [UIHint("Address")]
        public AddressViewModel InvoiceAddress { get; set; }

        [UIHint("Address")]
        public AddressViewModel ShippingAddress { get; set; }

        public bool ShippingAddressSupplied { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var address = InvoiceAddress;

            if (string.IsNullOrWhiteSpace(address.Address))
                yield return new ValidationResult("Address is a required field", new[] { "InvoiceAddress.Address" });

            if (string.IsNullOrWhiteSpace(address.Postcode))
                yield return new ValidationResult("Postcode is a required field", new[] { "InvoiceAddress.Postcode" });

            if (string.IsNullOrWhiteSpace(address.City))
                yield return new ValidationResult("City is a required field", new[] { "InvoiceAddress.City" });

            if (string.IsNullOrWhiteSpace(address.CountryCode))
                yield return new ValidationResult("Country code is a required field", new[] { "InvoiceAddress.CountryCode" });
        }

    }
}
