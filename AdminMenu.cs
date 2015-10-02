using System.Web.Routing;
using Orchard.Environment;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Cascade.WebShop
{
    public class AdminMenu : INavigationProvider
    {
        private readonly Work<RequestContext> _requestContextAccessor;
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public AdminMenu(Work<RequestContext> requestContextAccessor)
        {
            _requestContextAccessor = requestContextAccessor;
            T = NullLocalizer.Instance;
        }

        public void GetNavigation(NavigationBuilder builder)
        {
            var requestContext = _requestContextAccessor.Value;
            var idValue = (string)requestContext.RouteData.Values["id"];
            var id = 0;

            if (!string.IsNullOrEmpty(idValue))
            {
                int.TryParse(idValue, out id);
            }

            builder

                // Image set
                .AddImageSet("webshop")

                // "Webshop"
                .Add(T("Webshop"), "2", item => item

                    // default, but just in case they change it
                    .LinkToFirstChild(true) 

                    // "Customers"
                    .Add(T("Customers"), "2.0", m1 => m1
                        .Action("Index", "CustomerAdmin", new { area = "Cascade.WebShop" })
                        
                        // prevent the first tab from becoming the Action for Customers
                        .LinkToFirstChild(false)
                        
                        // Tabs
                        .Add(T("Details"), c => c.Action("Edit", "CustomerAdmin", new { id }).LocalNav())
                        .Add(T("Addresses"), c => c.Action("ListAddresses", "CustomerAdmin", new { id }).LocalNav())
                        .Add(T("Orders"), c => c.Action("ListOrders", "CustomerAdmin", new { id }).LocalNav())
                    )

                    // "Orders"
                    .Add(T("Orders"), "2.1", m2 => m2
                        .Action("Index", "OrderAdmin", new { area = "Cascade.WebShop" })

                        // prevent the first tab from becoming the Action for Orders
                        .LinkToFirstChild(false)
                        
                        // Tabs
                        .Add(T("Details"), o => o.Action("Edit", "OrderAdmin", new { id }).LocalNav())
                        .Add(T("Transaction History"), o => o.Action("History", "OrderAdmin", new { id }).LocalNav())
                    )
                );
        }
    }
}