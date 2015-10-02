using System;
using System.Collections.Generic;
using System.Globalization;

namespace Cascade.WebShop.Models
{
    public class OrderRecord
    {
        public virtual int Id { get; set; }
        public virtual int CustomerId { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual decimal SubTotal { get; set; }
        public virtual decimal GST { get; set; }
        public virtual OrderStatus Status { get; set; }
        public virtual string PaymentServiceProviderResponse { get; set; }
        public virtual string PaymentReference { get; set; }
        public virtual DateTime? PaidAt { get; set; }
        public virtual DateTime? CompletedAt { get; set; }
        public virtual DateTime? CancelledAt { get; set; }

        private IList<OrderDetailRecord> _details;
        public virtual IList<OrderDetailRecord> Details { 
            get { return _details ?? (_details = new List<OrderDetailRecord>());}
            set { _details = value; }
        }

        // Calculated field
        public virtual decimal Total
        {
            get { return SubTotal + GST; }
            set { }
        }

        // Calculated field
        public virtual string Number
        {
            get { return (Id + 1000).ToString(CultureInfo.InvariantCulture); }
            set { }
        }

        public virtual void UpdateTotals()
        {
            var subTotal = 0m;
            var vat = 0m;

            foreach (var detail in Details)
            {
                subTotal += detail.SubTotal;
                vat += detail.GST;
            }

            SubTotal = subTotal;
            GST = vat;
        }
    }
}