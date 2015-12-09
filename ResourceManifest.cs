using Orchard.UI.Resources;

namespace Cascade.WebShop
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            // Create and add a new manifest
            var manifest = builder.Add();

            // Define a "common" style sheet
            manifest.DefineStyle("Cascade.WebShop.Common").SetUrl("common.css");
            // Define the "shoppingcart" style sheet
            //manifest.DefineStyle("Cascade.WebShop.ShoppingCart").SetUrl("shoppingcart.css").SetDependencies("Cascade.WebShop.Common");
            // Define the Checkout Summary style sheet
            //manifest.DefineStyle("Cascade.WebShop.Checkout.Summary").SetUrl("checkout-summary.css").SetDependencies("Cascade.WebShop.Common");
            // Order
            manifest.DefineStyle("Cascade.WebShop.Order").SetUrl("order.css").SetDependencies("Cascade.WebShop.Common");
            // Simulated PSP
            manifest.DefineStyle("Cascade.WebShop.SimulatedPSP").SetUrl("simulated-psp.css").SetDependencies("Cascade.WebShop.Common");
            // define the shoppingcartwidget style sheet
            manifest.DefineStyle("Cascade.WebShop.ShoppingCartWidget").SetUrl("shoppingcartwidget.css").SetDependencies("Cascade.WebShop.Common");

            // Define Globalization resources
            manifest.DefineScript("Globalize").SetUrl("globalize.js").SetDependencies("jQuery");
            manifest.DefineScript("Globalize.Cultures").SetBasePath(manifest.BasePath + "scripts/cultures/").SetUrl("globalize.culture.js").SetCultures("en-US", "en-AU", "en-UK", "nl-NL").SetDependencies("Globalize", "jQuery");
            manifest.DefineScript("Globalize.SetCulture").SetUrl("~/Cascade.WebShop/Resource/SetCultureScript").SetDependencies("Globalize.Cultures");

            // Define the "shoppingcart" script and set a dependency on the "jQuery" resource
            manifest.DefineScript("Cascade.WebShop.ShoppingCart").SetUrl("shoppingcart.js").SetDependencies("jQuery", "jQuery_LinqJs", "Knockout");

            manifest.DefineScript("AddToShoppingCart").SetUrl("addtocartbutton.js").SetDependencies("jQuery");
            manifest.DefineScript("WebShop.Review").SetUrl("summary.js").SetDependencies("jQuery", "Knockout");

            manifest.DefineScript("Knockout").SetUrl("knockout-3.3.0.js", "knockout-3.3.0.debug.js");
        }
    }
}