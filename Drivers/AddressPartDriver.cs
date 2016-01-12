using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Cascade.WebShop.Models;
using Orchard.ContentManagement.Handlers;
using System;

namespace Cascade.WebShop.Drivers
{
    public class AddressPartDriver : ContentPartDriver<AddressPart>
    {
        private readonly IContentManager _contentManager;

        public AddressPartDriver(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }
        
        protected override string Prefix
        {
            get { return "Address"; }
        }

        protected override DriverResult Editor(AddressPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Address_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/Address", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(AddressPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        protected override void Importing(AddressPart part, ImportContentContext context)
        {
            // Don't do anything if the tag is not specified.
            if (context.Data.Element(part.PartDefinition.Name) == null)
            {
                return;
            }

            context.ImportAttribute(part.PartDefinition.Name, "Customer", customer =>
            {
                var contentItem = context.GetItemFromSession(customer);
                if (contentItem != null)
                {
                    part.CustomerId = contentItem.Id;
                }
            });

            context.ImportAttribute(part.PartDefinition.Name, "Address", address => part.Address = address);
            context.ImportAttribute(part.PartDefinition.Name, "City", city => part.City= city);
            context.ImportAttribute(part.PartDefinition.Name, "CountryCode", countryCode => part.CountryCode = countryCode);
            context.ImportAttribute(part.PartDefinition.Name, "Name", name => part.Name = name);
            context.ImportAttribute(part.PartDefinition.Name, "Postcode", postcode => part.Postcode = postcode);
            context.ImportAttribute(part.PartDefinition.Name, "State", state => part.State = state);
            context.ImportAttribute(part.PartDefinition.Name, "Type", type => part.Type = type);
        }

        protected override void Exporting(AddressPart part, ExportContentContext context)
        {
            var customer = _contentManager.Get(part.CustomerId);
            if (customer != null)
            {
                var identity = _contentManager.GetItemMetadata(customer).Identity;
                context.Element(part.PartDefinition.Name).SetAttributeValue("Customer", identity.ToString());
            }

            context.Element(part.PartDefinition.Name).SetAttributeValue("Id", part.Id);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Address", part.Address);
            context.Element(part.PartDefinition.Name).SetAttributeValue("City", part.City);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CountryCode", part.CountryCode);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Name", part.Name);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Postcode", part.Postcode);
            context.Element(part.PartDefinition.Name).SetAttributeValue("State", part.State);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Type", part.Type);

        }
    }
}