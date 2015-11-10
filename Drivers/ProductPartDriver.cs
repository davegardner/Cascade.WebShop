using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Cascade.WebShop.Models;
using Cascade.WebShop.Services;

namespace Cascade.WebShop.Drivers
{
    public class ProductPartDriver : ContentPartDriver<ProductPart>
    {

        private readonly IProductService _service;
        public ProductPartDriver(IProductService service)
        {
            _service = service;
                 
        }

        protected override string Prefix
        {
            get { return "Product"; }
        }

        protected override DriverResult Display(ProductPart part, string displayType, dynamic shapeHelper)
        {
            return Combined(
                 ContentShape("Parts_Product_SummaryAdmin", () => shapeHelper.Parts_Product_SummaryAdmin(
                     Price: part.UnitPrice,
                     Sku: part.Sku,
                     InStock: part.InStock,
                     UseStockControl: part.UseStockControl,
                     NumberSold: part.NumberSold,
                     CanReorder: part.CanReorder,
                     ReorderLevel: part.ReorderLevel
                 )),
                 ContentShape("Parts_Product", () => shapeHelper.Parts_Product(
                     Price: part.UnitPrice,
                     Sku: part.Sku
                 )),
                  ContentShape("Parts_Product_AddButton", () => shapeHelper.Parts_Product_AddButton(
                      ProductId: part.Id
                      ))
                 );
        }

        protected override DriverResult Editor(ProductPart part, dynamic shapeHelper)
        {
            if (string.IsNullOrWhiteSpace(part.Sku))
                part.Sku = _service.GetNextSku();

            return ContentShape("Parts_Product_Edit", () => shapeHelper
                .EditorTemplate(TemplateName: "Parts/Product", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(ProductPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

     }
}