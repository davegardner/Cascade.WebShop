using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Cascade.WebShop.Models;
using Orchard.Security;
using Orchard.ContentManagement.Handlers;
using System;
using Cascade.WebShop.Services;
using System.Xml;

namespace Cascade.WebShop.Drivers
{
    public class CustomerPartDriver : ContentPartDriver<CustomerPart>
    {
        protected override string Prefix
        {
            get { return "Customer"; }
        }

        protected override DriverResult Editor(CustomerPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Customer_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/Customer", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(CustomerPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);

            // There are problems with updating the user email:
            // 1. The following code doesn't work and I do not know why, see AdminController.EditPOST() for an example of code that does work
            // 2. If the email CAN be made to work then need to deal with preserving uniqueness of email.
            // 3. And whata about the fact that email and username are set the same? Is this essential for Customers? Is it OK if they differ?
            //var user = part.User;
            //updater.TryUpdateModel(user, "User", null, null);

            return Editor(part, shapeHelper);
        }
        protected override void Importing(CustomerPart part, ImportContentContext context)
        {
            // Don't do anything if the tag is not specified.
            if (context.Data.Element(part.PartDefinition.Name) == null)
            {
                return;
            }

            context.ImportAttribute(part.PartDefinition.Name, "FirstName", firstName => part.FirstName = firstName);
            context.ImportAttribute(part.PartDefinition.Name, "LastName", lastName => part.LastName = lastName);
            context.ImportAttribute(part.PartDefinition.Name, "Title", title => part.Title = title);
            context.ImportAttribute(part.PartDefinition.Name, "SubscribeToMailingList", subscribe => part.SubscribeToMailingList = Convert.ToBoolean(subscribe));
            context.ImportAttribute(part.PartDefinition.Name, "AgreeTermsAndConditions", agree => part.AgreeTermsAndConditions = Convert.ToBoolean(agree));
            context.ImportAttribute(part.PartDefinition.Name, "WelcomeEmailPending", welcome => part.WelcomeEmailPending = Convert.ToBoolean(welcome));
            context.ImportAttribute(part.PartDefinition.Name, "ReceivePost", receivePost => part.ReceivePost = Convert.ToBoolean(receivePost));

            // Dates must be declared nullable in the Model or content items will fail to be created during import
            context.ImportAttribute(part.PartDefinition.Name, "CreatedUtc", createdUtc =>
                part.CreatedUtc = XmlConvert.ToDateTime(createdUtc, XmlDateTimeSerializationMode.Utc));
        }

        protected override void Exporting(CustomerPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("FirstName", part.FirstName);
            context.Element(part.PartDefinition.Name).SetAttributeValue("LastName", part.LastName);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Title", part.Title);
            context.Element(part.PartDefinition.Name).SetAttributeValue("SubscribeToMailingList", part.SubscribeToMailingList);
            context.Element(part.PartDefinition.Name).SetAttributeValue("AgreeTermsAndConditions", part.AgreeTermsAndConditions);
            context.Element(part.PartDefinition.Name).SetAttributeValue("WelcomEmailPending", part.WelcomeEmailPending);
            context.Element(part.PartDefinition.Name).SetAttributeValue("ReceivePost", part.ReceivePost);
            if (part.CreatedUtc != null)
            {
                context.Element(part.PartDefinition.Name)
                    .SetAttributeValue("CreatedUtc", XmlConvert.ToString(part.CreatedUtc.Value, XmlDateTimeSerializationMode.Utc));
            }

            // a 1-n relationship
            // first, get the ContentPart element
            //var parent = context.Element(part.PartDefinition.Name);
            // create the container element

            //context.Element(part.PartDefinition.Name).SetElementValue("addresses", "");
            //var addresses = context.Element(part.PartDefinition.Name).Element("addresses");

            //foreach (var address in _customerService.GetAddresses(part.Id))
            //{
            //    // every node must have a unique name, else SetElementValue won't work
            //    var elid = "address" + address.Id;
            //    // again, first create the container element
            //    addresses.SetElementValue(elid, "");
            //    var el = addresses.Element(elid);
            //    // then import the values (without Id, it will be autogenerated)
            //    //el.SetAttributeValue("Id", offer.Id);
            //    el.SetAttributeValue("Type", address.Type);
            //    el.SetAttributeValue("Name", address.Name);
            //    el.SetAttributeValue("City", address.City);
            //    el.SetAttributeValue("State", address.State);
            //    el.SetAttributeValue("CountryCode", address.CountryCode);
            //    el.SetAttributeValue("Postcode", address.Postcode);

            //    //TODO: Order
            //}
        }
    }
}