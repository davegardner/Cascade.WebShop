using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Cascade.WebShop.Models;
using Orchard.ContentManagement.Handlers;
using System;

namespace Cascade.WebShop.Drivers
{
    public class ShippingProductPartDriver : ContentPartDriver<ShippingProductPart>
    {

        protected override string Prefix
        {
            get { return "ShippingProduct"; }
        }

        protected override DriverResult Display(ShippingProductPart part, string displayType, dynamic shapeHelper)
        {
            return 
                 ContentShape("Parts_ShippingProduct", () => shapeHelper.Parts_ShippingProduct(
                     Title: part.Title,
                     Description: part.Description
                 )
                 );
        }

        protected override DriverResult Editor(ShippingProductPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_ShippingProduct_Edit", () => shapeHelper
                .EditorTemplate(TemplateName: "Parts/ShippingProduct", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(ShippingProductPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
        protected override void Importing(ShippingProductPart part, ImportContentContext context)
        {
            // Don't do anything if the tag is not specified.
            if (context.Data.Element(part.PartDefinition.Name) == null)
            {
                return;
            }

            context.ImportAttribute(part.PartDefinition.Name, "Title", title => part.Title = title);
            context.ImportAttribute(part.PartDefinition.Name, "Description", description => part.Description = description);
        }

        protected override void Exporting(ShippingProductPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Title", part.Record.Title);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Description", part.Record.Description);

        }
    }
}