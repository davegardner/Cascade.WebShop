using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Cascade.WebShop.Models;
using Cascade.WebShop.Services;
using Orchard.ContentManagement.Handlers;
using System;

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

        protected override void Importing(ProductPart part, ImportContentContext context)
        {
            // Don't do anything if the tag is not specified.
            if (context.Data.Element(part.PartDefinition.Name) == null)
            {
                return;
            }

            context.ImportAttribute(part.PartDefinition.Name, "InStock", inStock => part.InStock = Convert.ToInt32(inStock));
            context.ImportAttribute(part.PartDefinition.Name, "IsShippable", isShippable => part.IsShippable = Convert.ToBoolean(isShippable));
            context.ImportAttribute(part.PartDefinition.Name, "NumberSold", numberSold => part.NumberSold = Convert.ToInt32(numberSold));
            context.ImportAttribute(part.PartDefinition.Name, "ReorderLevel", reorderLevel => part.ReorderLevel = Convert.ToInt32(reorderLevel));
            context.ImportAttribute(part.PartDefinition.Name, "Sku", sku => part.Sku = sku);
            context.ImportAttribute(part.PartDefinition.Name, "UnitPrice", unitPrice => part.UnitPrice = Convert.ToDecimal(unitPrice));
            context.ImportAttribute(part.PartDefinition.Name, "UseStockControl", useStockControl => part.UseStockControl = Convert.ToBoolean(useStockControl));
            context.ImportAttribute(part.PartDefinition.Name, "CanReorder", canReorder => part.CanReorder = Convert.ToBoolean(canReorder));
        }

        protected override void Exporting(ProductPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("InStock", part.Record.InStock);
            context.Element(part.PartDefinition.Name).SetAttributeValue("IsShippable", part.Record.IsShippable);
            context.Element(part.PartDefinition.Name).SetAttributeValue("NumberSold", part.Record.NumberSold);
            context.Element(part.PartDefinition.Name).SetAttributeValue("ReorderLevel", part.Record.ReorderLevel);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Sku", part.Record.Sku);
            context.Element(part.PartDefinition.Name).SetAttributeValue("UnitPrice", part.Record.UnitPrice);
            context.Element(part.PartDefinition.Name).SetAttributeValue("UseStockControl", part.Record.UseStockControl);
            context.Element(part.PartDefinition.Name).SetAttributeValue("CanReorder", part.Record.CanReorder);

        }
     }
}