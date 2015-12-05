using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using System.Globalization;
using Orchard.Core.Common.Utilities;

namespace Cascade.WebShop.Models
{
    public class OrderRecordPart: ContentPart<OrderRecord>
    {
        public OrderRecordPart()
        {
            Details = new List<OrderDetail>();
        }

        public int CustomerId { get { return Retrieve(r => r.CustomerId); } set { Store(r => r.CustomerId, value); } }
        public DateTime? CreatedAt { get { return Retrieve(r => r.CreatedAt); } set { Store(r => r.CreatedAt, value); } }
        public decimal SubTotal { get { return Retrieve(r => r.SubTotal); } set { Store(r => r.SubTotal, value); } }
        public decimal GST { get { return Retrieve(r => r.GST); } set { Store(r => r.GST, value); } }
        public OrderStatus Status { get { return Retrieve(r => r.Status); } set { Store(r => r.Status, value); } }
        public string PaymentServiceProviderResponse { get { return Retrieve(r => r.PaymentServiceProviderResponse); } set { Store(r => r.PaymentServiceProviderResponse, value); } }
        public string PaymentReference { get { return Retrieve(r => r.PaymentReference); } set { Store(r => r.PaymentReference, value); } }
        public DateTime? PaidAt { get { return Retrieve(r => r.PaidAt); } set { Store(r => r.PaidAt, value); } }
        public DateTime? CompletedAt { get { return Retrieve(r => r.CompletedAt); } set { Store(r => r.CompletedAt, value); } }
        public DateTime? CancelledAt { get { return Retrieve(r => r.CancelledAt); } set { Store(r => r.CancelledAt, value); } }

        //private readonly LazyField<IList<OrderDetailPart>> _details = new LazyField<IList<OrderDetailPart>>();

        //public LazyField<IList<OrderDetailPart>> DetailsField
        //{
        //    get { return _details; }
        //}

        //public IList<OrderDetailPart> Details { get { return DetailsField.Value; } }


        public IList<OrderDetail> Details
        {
            get;
            set;
        }

        public string RawDetails {
            get { return Retrieve(x => x.RawDetails); }
            set { Store(x => x.RawDetails, value); }
        }
      
        // Calculated field
        public  decimal Total
        {
            get { return SubTotal + GST; }
            set { }
        }

        // Calculated field
        public  string Number
        {
            get { return (Id + 1000).ToString(CultureInfo.InvariantCulture); }
            set { }
        }

        public  void UpdateTotals()
        {
            //Record.UpdateTotals();
        }
    }
}