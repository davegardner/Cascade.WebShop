using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Cascade.WebShop.Models;
using Cascade.WebShop.Services;

namespace Cascade.WebShop.Drivers
{
    public class OrderRecordPartDriver : ContentPartDriver<OrderRecordPart>
    {

        //private readonly IOrderRecordService _service;
        //public OrderRecordPartDriver(IOrderRecordService service)
        //{
        //    _service = service;

        //}

        protected override string Prefix
        {
            get { return "OrderRecord"; }
        }

        protected override DriverResult Display(OrderRecordPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_OrderRecord", () => shapeHelper.Parts_OrderRecord(
                     OrderRecordPart: part
                     )
                );

            //    return Combined(
            //         ContentShape("Parts_OrderRecord_SummaryAdmin", () => shapeHelper.Parts_OrderRecord_SummaryAdmin(
            //             OrderRecordPart: part
            //             //Price: part.UnitPrice,
            //             //Sku: part.Sku,
            //             //InStock: part.InStock,
            //             //UseStockControl: part.UseStockControl,
            //             //NumberSold: part.NumberSold,
            //             //CanReorder: part.CanReorder,
            //             //ReorderLevel: part.ReorderLevel
            //         )),
            //         ContentShape("Parts_OrderRecord", () => shapeHelper.Parts_OrderRecord(
            //             Price: part.UnitPrice,
            //             Sku: part.Sku
            //         )),
            //          ContentShape("Parts_OrderRecord_AddButton", () => shapeHelper.Parts_OrderRecord_AddButton(
            //              OrderRecordId: part.Id
            //              ))
            //         );
        }

        protected override DriverResult Editor(OrderRecordPart part, dynamic shapeHelper)
        {
            //if (string.IsNullOrWhiteSpace(part.Sku))
            //    part.Sku = _service.GetNextSku();

            return ContentShape("Parts_OrderRecord_Edit", () => shapeHelper
                .EditorTemplate(TemplateName: "Parts/OrderRecord", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(OrderRecordPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

    }
}