using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Cascade.WebShop.Models;

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

    }
}