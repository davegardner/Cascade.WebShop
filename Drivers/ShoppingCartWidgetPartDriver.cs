using System.Linq;
using Orchard.ContentManagement.Drivers;
using Cascade.WebShop.Models;
using Cascade.WebShop.Services;
using Orchard.Data;

namespace Cascade.WebShop.Drivers
{
    public class ShoppingCartWidgetPartDriver : ContentPartDriver<ShoppingCartWidgetPart>
    {
        private readonly IShoppingCart _shoppingCart;
        private readonly IWebshopSettingsService _webshopSettings;

        public ShoppingCartWidgetPartDriver(IShoppingCart shoppingCart, IWebshopSettingsService webShopSettings)
        {
            _shoppingCart = shoppingCart;
            _webshopSettings = webShopSettings;
        }

        protected override DriverResult Display(ShoppingCartWidgetPart part, string displayType, dynamic shapeHelper)
        {

            return ContentShape("Parts_ShoppingCartWidget", () => shapeHelper.Parts_ShoppingCartWidget(
                ItemCount: _shoppingCart.ItemCount(),
                TotalAmount: _shoppingCart.Total(),
                ContinueShoppingUrl: _webshopSettings.GetContinueShoppingUrl()
            ));
        }
    }
}