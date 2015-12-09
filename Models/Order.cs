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

        //SAFETEC
        public virtual bool Monday { get; set; }
        public virtual bool Tuesday { get; set; }
        public virtual bool Wednesday { get; set; }
        public virtual bool Thursday { get; set; }
        public virtual bool Friday { get; set; }
        public virtual bool Saturday { get; set; }
        public virtual bool Sunday { get; set; }

        public virtual bool Morning { get; set; }
        public virtual bool Afternoon { get; set; }
        public virtual bool Evening { get; set; }

        public virtual DateTime? SpecificDateTime { get; set; }

        public virtual string Notes { get; set; }
    }

    // PART
    public class OrderPart : ContentPart<OrderRecord>
    {
        public OrderPart()
        {
            Details = new List<OrderDetail>();
        }

        // column storage
        public int CustomerId { get { return Retrieve(r => r.CustomerId); } set { Store(r => r.CustomerId, value); } }
        public OrderStatus Status { get { return Retrieve(r => r.Status); } set { Store(r => r.Status, value); } }
        public DateTime? CreatedAt { get { return Retrieve(r => r.CreatedAt); } set { Store(r => r.CreatedAt, value); } }

        // infoset storage only
        public decimal SubTotal { get { return this.Retrieve(r => r.SubTotal); } set { this.Store(r => r.SubTotal, value); } }
        public decimal GST { get { return this.Retrieve(r => r.GST); } set { this.Store(r => r.GST, value); } }
        public string PaymentServiceProviderResponse { get { return this.Retrieve(r => r.PaymentServiceProviderResponse); } set { this.Store(r => r.PaymentServiceProviderResponse, value); } }
        public string PaymentReference { get { return this.Retrieve(r => r.PaymentReference); } set { this.Store(r => r.PaymentReference, value); } }
        public DateTime? PaidAt { get { return this.Retrieve(r => r.PaidAt); } set { this.Store(r => r.PaidAt, value); } }
        public DateTime? CompletedAt { get { return this.Retrieve(r => r.CompletedAt); } set { this.Store(r => r.CompletedAt, value); } }
        public DateTime? CancelledAt { get { return this.Retrieve(r => r.CancelledAt); } set { this.Store(r => r.CancelledAt, value); } }
        public string RawDetails
        {
            get { return this.Retrieve(x => x.RawDetails); }
            set { this.Store(x => x.RawDetails, value); }
        }

        // Derived field
        public IList<OrderDetail> Details
        {
            get;
            set;
        }


        // Calculated field
        public decimal Total
        {
            get { return SubTotal + GST; }
            set { }
        }

        // Calculated field
        public string Number
        {
            get { return (Id + 1000).ToString(CultureInfo.InvariantCulture); }
            set { }
        }

        // utility
        public void UpdateTotals()
        {
            SubTotal = Details.Sum(d => d.SubTotal);
            GST = SubTotal / 11;
            RawDetails = OrderDetailSerializer.Serialize(Details);
        }

        //SAFETEC
        public bool Monday
        {
            get { return this.Retrieve(x => x.Monday); }
            set { this.Store(x => x.Monday, value); }
        }
        public bool Tuesday
        {
            get { return this.Retrieve(x => x.Tuesday); }
            set { this.Store(x => x.Tuesday, value); }
        }
        public bool Wednesday
        {
            get { return this.Retrieve(x => x.Wednesday); }
            set { this.Store(x => x.Wednesday, value); }
        }
        public bool Thursday
        {
            get { return this.Retrieve(x => x.Thursday); }
            set { this.Store(x => x.Thursday, value); }
        }
        public bool Friday
        {
            get { return this.Retrieve(x => x.Friday); }
            set { this.Store(x => x.Friday, value); }
        }
        public bool Saturday
        {
            get { return this.Retrieve(x => x.Saturday); }
            set { this.Store(x => x.Saturday, value); }
        }
        public bool Sunday
        {
            get { return this.Retrieve(x => x.Sunday); }
            set { this.Store(x => x.Sunday, value); }
        }

        public bool Morning
        {
            get { return this.Retrieve(x => x.Morning); }
            set { this.Store(x => x.Morning, value); }
        }
        public bool Afternoon
        {
            get { return this.Retrieve(x => x.Afternoon); }
            set { this.Store(x => x.Afternoon, value); }
        }
        public bool Evening
        {
            get { return this.Retrieve(x => x.Evening); }
            set { this.Store(x => x.Evening, value); }
        }
        public DateTime? SpecificDateTime
        {
            get { return this.Retrieve(x => x.SpecificDateTime); }
            set { this.Store(x => x.SpecificDateTime, value); }
        }
        
        public string Notes
        {
            get { return this.Retrieve(x => x.Notes); }
            set { Store(x => x.Notes, value); }
        }

    }
}