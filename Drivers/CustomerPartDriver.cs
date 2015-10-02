using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Cascade.WebShop.Models;
using Orchard.Security;

namespace Cascade.WebShop.Drivers
{
    public class CustomerPartDriver : ContentPartDriver<CustomerPart>
    {
        protected override string Prefix
        {
            get { return "Customer"; }
        }

        protected override DriverResult Editor(CustomerPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Customer_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/Customer", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(CustomerPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);

            // There are problems with updating the user email:
            // 1. The following code doesn't work and I do not know why, see AdminController.EditPOST() for an example of code that does work
            // 2. If the email CAN be made to work then need to deal with preserving uniqueness of email.
            // 3. And whata about the fact that email and username are set the same? Is this essential for Customers? Is it OK if they differ?
            //var user = part.User;
            //updater.TryUpdateModel(user, "User", null, null);

            return Editor(part, shapeHelper);
        }
    }
}