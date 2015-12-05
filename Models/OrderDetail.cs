namespace Cascade.WebShop.Models
{
    public class OrderDetail
    {
        public bool Deleted { get; set; }
        public int Id { get; set; }
        public int OrderRecord_Id { get; set; }
        public int ProductPartRecord_Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal GSTRate { get; set; }
        public string Description { get; set; }
        public string Sku { get; set; }


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

    //public class OrderDetailPart : ContentPart<OrderDetailRecord>
    //{
    //    public int OrderRecord_Id
    //    {
    //        get { return Retrieve(x => x.OrderRecord_Id); }
    //        set { Store(x => x.OrderRecord_Id, value); }
    //    }
    //    public  int ProductPartRecord_Id
    //    {
    //        get { return Retrieve(x => x.ProductPartRecord_Id); }
    //        set { Store(x => x.ProductPartRecord_Id, value); }
    //    }

    //    public  int Quantity
    //    {
    //        get { return Retrieve(x => x.Quantity); }
    //        set { Store(x => x.Quantity, value); }
    //    }

    //    public  decimal UnitPrice
    //    {
    //        get { return Retrieve(x => x.UnitPrice); }
    //        set { Store(x => x.UnitPrice, value); }
    //    }

    //    public  decimal GSTRate
    //    {
    //        get { return Retrieve(x => x.GSTRate); }
    //        set { Store(x => x.GSTRate, value); }
    //    }

    //    public  string Description
    //    {
    //        get { return Retrieve(x => x.Description); }
    //        set { Store(x => x.Description, value); }
    //    }

    //    public string Sku
    //    {
    //        get { return Retrieve(x => x.Sku); }
    //        set { Store(x => x.Sku, value); }
    //    }



    //    // Calculated field
    //    public  decimal UnitGST
    //    {
    //        get { return UnitPrice * GSTRate; }
    //        set { ;}
    //    }

    //    // Calculated field
    //    public  decimal GST
    //    {
    //        get { return UnitGST * Quantity; }
    //        set { ;}
    //    }

    //    // Calculated field
    //    public  decimal SubTotal
    //    {
    //        get { return UnitPrice * Quantity; }
    //        set { ;}
    //    }

    //    // Calculated field
    //    public  decimal Total
    //    {
    //        get { return SubTotal + GST; }
    //        set { ;}
    //    }


    //}

}