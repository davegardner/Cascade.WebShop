using Cascade.WebShop.Helpers;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Cascade.WebShop.Models
{
    // RECORD
    public class OrderRecord : ContentPartRecord
    {
        public virtual int CustomerId { get; set; }
        public virtual DateTime? CreatedAt { get; set; }
        public virtual decimal SubTotal { get; set; }
        public virtual decimal GST { get; set; }
        public virtual OrderStatus Status { get; set; }
        public virtual string PaymentServiceProviderResponse { get; set; }
        public virtual string PaymentReference { get; set; }
        public virtual DateTime? PaidAt { get; set; }
        public virtual DateTime? CompletedAt { get; set; }
        public virtual DateTime? CancelledAt { get; set; }
        public virtual string RawDetails { get; set; }

    }

    // PART
    public class OrderPart: ContentPart<OrderRecord>
    {
        public OrderPart()
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
            SubTotal = Details.Sum(d => d.SubTotal);
            GST = SubTotal / 11;
            RawDetails = OrderDetailSerializer.Serialize(Details);
        }
    }
}