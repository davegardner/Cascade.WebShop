namespace Cascade.WebShop.Models
{
    public class OrderDetailRecord
    {
        public virtual int Id { get; set; }
        public virtual int OrderRecord_Id { get; set; }
        public virtual int ProductPartRecord_Id { get; set; }
        public virtual int Quantity { get; set; }
        public virtual decimal UnitPrice { get; set; }
        public virtual decimal GSTRate { get; set; }
        public virtual string Description { get; set; }
        public virtual string Sku { get; set; }


        // Calculated field
        public virtual decimal UnitGST
        {
            get { return UnitPrice * GSTRate; }
            set { ;}
        }

        // Calculated field
        public virtual decimal GST
        {
            get { return UnitGST * Quantity; }
            set { ;}
        }

        // Calculated field
        public virtual decimal SubTotal
        {
            get { return UnitPrice * Quantity; }
            set { ;}
        }

        // Calculated field
        public virtual decimal Total
        {
            get { return SubTotal + GST; }
            set { ;}
        }

    }
}