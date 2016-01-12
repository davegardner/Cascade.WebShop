using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Cascade.WebShop.Models;
using Cascade.WebShop.Services;
using Orchard.ContentManagement.Handlers;
using System;

namespace Cascade.WebShop.Drivers
{
    public class OrderRecordPartDriver : ContentPartDriver<OrderPart>
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

        protected override DriverResult Display(OrderPart part, string displayType, dynamic shapeHelper)
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

        protected override DriverResult Editor(OrderPart part, dynamic shapeHelper)
        {
            //if (string.IsNullOrWhiteSpace(part.Sku))
            //    part.Sku = _service.GetNextSku();

            return ContentShape("Parts_OrderRecord_Edit", () => shapeHelper
                .EditorTemplate(TemplateName: "Parts/OrderRecord", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(OrderPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        // IF YOU NEED IMPORT EXPORT THEN YOU NEED TO IMPLEMENT ORDER DETAILS.
        // OrderDetail points to Customer and Product so you need to resolve identity for these.
        // For now I have disabled OrderPart Import/Export as I do not think it is useful in reality.



        //protected override void Importing(OrderPart part, ImportContentContext context)
        //{
        //    // Don't do anything if the tag is not specified.
        //    if (context.Data.Element(part.PartDefinition.Name) == null)
        //    {
        //        return;
        //    }

        //    context.ImportAttribute(part.PartDefinition.Name, "CreatedAt", created => part.CreatedAt= Convert.ToDateTime(created));
        //    context.ImportAttribute(part.PartDefinition.Name, "SubTotal", subTotal => part.SubTotal = Convert.ToDecimal(subTotal));
        //    context.ImportAttribute(part.PartDefinition.Name, "GST", gst => part.GST= Convert.ToDecimal(gst));
        //    context.ImportAttribute(part.PartDefinition.Name, "Status", status => part.Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), status));
        //    context.ImportAttribute(part.PartDefinition.Name, "PaymentServiceProviderResponse", response => part.PaymentServiceProviderResponse = response);
        //    context.ImportAttribute(part.PartDefinition.Name, "PaymentReference", reference => part.PaymentReference = reference);
        //    context.ImportAttribute(part.PartDefinition.Name, "PaidAt", paidAt => part.PaidAt = Convert.ToDateTime(paidAt));
        //    context.ImportAttribute(part.PartDefinition.Name, "CompletedAt", completed => part.CompletedAt= Convert.ToDateTime(completed));
        //    context.ImportAttribute(part.PartDefinition.Name, "CancelledAt", cancelled => part.CancelledAt = Convert.ToDateTime(cancelled));
        //    context.ImportAttribute(part.PartDefinition.Name, "RawDetails", rawDetails => part.RawDetails = rawDetails);

        //    context.ImportAttribute(part.PartDefinition.Name, "Morning", morning => part.Morning = Convert.ToBoolean(morning));
        //    context.ImportAttribute(part.PartDefinition.Name, "Afternoon", afternoon => part.Afternoon = Convert.ToBoolean(afternoon));
        //    context.ImportAttribute(part.PartDefinition.Name, "Evening", evening => part.Evening = Convert.ToBoolean(evening));
            
        //    context.ImportAttribute(part.PartDefinition.Name, "Monday", monday => part.Monday = Convert.ToBoolean(monday));
        //    context.ImportAttribute(part.PartDefinition.Name, "Tuesday", tuesday => part.Tuesday = Convert.ToBoolean(tuesday));
        //    context.ImportAttribute(part.PartDefinition.Name, "Wednesday", wednesday => part.Wednesday = Convert.ToBoolean(wednesday));
        //    context.ImportAttribute(part.PartDefinition.Name, "Thursday", thursday => part.Thursday = Convert.ToBoolean(thursday));
        //    context.ImportAttribute(part.PartDefinition.Name, "Friday", friday => part.Friday = Convert.ToBoolean(friday));
        //    context.ImportAttribute(part.PartDefinition.Name, "Saturday", saturday => part.Saturday = Convert.ToBoolean(saturday));
        //    context.ImportAttribute(part.PartDefinition.Name, "Sunday", sunday => part.Sunday = Convert.ToBoolean(sunday));

        //    context.ImportAttribute(part.PartDefinition.Name, "SpecificDateTime", specific => part.SpecificDateTime = Convert.ToDateTime(specific));
        //    context.ImportAttribute(part.PartDefinition.Name, "Notes", notes => part.Notes = notes);
        //}

        //protected override void Exporting(OrderPart part, ExportContentContext context)
        //{
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("CreatedAt", part.Record.CreatedAt);
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("SubTotal", part.Record.SubTotal);
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("GST", part.Record.GST);
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("Status", part.Record.Status.ToString());
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("PaymentServiceProviderResponse", part.Record.PaymentServiceProviderResponse);
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("PaymentReference", part.Record.PaymentReference);
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("PaidAt", part.Record.PaidAt);
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("CompletedAt", part.Record.CompletedAt);
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("CancelledAt", part.Record.CancelledAt);
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("RawDetails", part.Record.RawDetails);

        //    context.Element(part.PartDefinition.Name).SetAttributeValue("Monday", part.Record.Monday);
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("Tuesday", part.Record.Tuesday);
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("Wednesday", part.Record.Wednesday);
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("Thursday", part.Record.Thursday);
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("Friday", part.Record.Friday);
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("Saturday", part.Record.Saturday);
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("Sunday", part.Record.Sunday);

        //    context.Element(part.PartDefinition.Name).SetAttributeValue("Morning", part.Record.Morning);
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("Afternoon", part.Record.Afternoon);
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("Evening", part.Record.Evening);
            
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("SpecificDateTime", part.Record.SpecificDateTime);
        //    context.Element(part.PartDefinition.Name).SetAttributeValue("Notes", part.Record.Notes);

        //}


    }
}