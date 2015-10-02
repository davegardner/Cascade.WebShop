using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement;
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.Aspects;

namespace Cascade.WebShop.Models
{
    public class ShippingProductRecord : ContentPartRecord
    {
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
    }

    public class ShippingProductPart : ContentPart<ShippingProductRecord>, ITitleAspect
    {

        [Required]
        public string Title
        {
            get { return Retrieve(r=>r.Title); }
            set { Store(r=>r.Title, value); }
        }

        public string Description
        {
            get { return Retrieve(r=>r.Description); }
            set { Store(r=>r.Description, value); }
        }

    }
}