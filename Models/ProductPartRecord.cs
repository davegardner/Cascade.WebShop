using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Cascade.WebShop.Models
{
    public class ProductRecord : ContentPartRecord
    {
        public virtual decimal UnitPrice { get; set; }
        public virtual string Sku { get; set; }
        public virtual int InStock { get; set; }
        public virtual int NumberSold  { get; set; }
        public virtual bool UseStockControl { get; set; }
        public virtual bool CanReorder { get; set; }
        public virtual int ReorderLevel { get; set; }
        public virtual bool IsShippable { get; set; }
    }

    public class ProductPart : ContentPart<ProductRecord>
    {

        public decimal UnitPrice
        {
            get { return Retrieve(r=>r.UnitPrice); }
            set { Store(r=>r.UnitPrice, value); }
        }

        public string Sku
        {
            get { return Retrieve(r=>r.Sku); }
            set { Store(r=>r.Sku, value); }
        }

        public bool UseStockControl
        {
            get { return Retrieve(r => r.UseStockControl); }
            set { Store(r => r.UseStockControl, value); }
        }
        
        public int InStock 
        {
            get { return Retrieve(r=>r.InStock); }
            set { Store(r=>r.InStock, value); }
        }

        public int NumberSold
        {
            get { return Retrieve(r=>r.NumberSold); }
            set { Store(r=>r.NumberSold, value); }
        }

        public bool CanReorder
        {
            get { return Retrieve(r=>r.CanReorder); }
            set { Store(r=>r.CanReorder, value); }
        }

        public int ReorderLevel
        {
            get { return Retrieve(r=>r.ReorderLevel); }
            set { Store(r=>r.ReorderLevel, value); }
        }


        public bool IsShippable
        {
            get { return Retrieve(r=>r.IsShippable); }
            set { Store(r=>r.IsShippable, value); }
        }
    }
}
