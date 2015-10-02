using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Cascade.WebShop.Models;
using Cascade.WebShop.Services;
using Cascade.WebShop.ViewModels;
using Orchard;
using Orchard.Mvc;
using Orchard.Themes;

namespace Cascade.WebShop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCart _shoppingCart;
        private readonly IOrchardServices _services;
        private readonly IWebshopSettingsService _webshopSettings;

        public ShoppingCartController(IShoppingCart shoppingCart, IOrchardServices services, IWebshopSettingsService webshopSettings)
        {
            _shoppingCart = shoppingCart;
            _services = services;
            _webshopSettings = webshopSettings;
        }

        [HttpPost]
        public ActionResult Add(int id)
        {
            // Add the specified content id to the shopping cart with a quantity of 1.
            _shoppingCart.Add(id, 1);

            // Redirect the user to the Index action
            return RedirectToAction("Index");
        }

        [Themed]
        public ActionResult Index()
        {
            // Create a new shape using the "New" property of IOrchardServices.
            var shape = _services.New.ShoppingCart(
                Products: _shoppingCart.GetProducts().Select(p => _services.New.ShoppingCartItem(
                    ProductPart: p.ProductPart,
                    Quantity: p.Quantity,
                    Type: p.ProductPart.TypeDefinition.Name,
                    Title: _services.ContentManager.GetItemMetadata(p.ProductPart).DisplayText)
                ).ToList(),
                Total: _shoppingCart.Total(),
                Subtotal: _shoppingCart.Subtotal(),
                GST: _shoppingCart.GST(),
                ContinueShoppingUrl: _webshopSettings.GetContinueShoppingUrl()
            );

            // Return a ShapeResult
            return new ShapeResult(this, shape);
        }

        [HttpPost]
        public ActionResult AddToCart(string productId)
        {
            int id = 0;
            int.TryParse(productId, out id);

            // Add the specified content id to the shopping cart with a quantity of 1.
            if (id > 0)
                _shoppingCart.Add(id, 1);

            return Json(true);
        }

        [HttpPost]
        public ActionResult Update(string command, UpdateShoppingCartItemViewModel[] items)
        {
            UpdateShoppingCart(items);

            if (Request.IsAjaxRequest())
                return Json(true);

            switch (command)
            {
                case "Checkout":
                    return RedirectToAction("SignupOrLogin", "Checkout");
                case "ContinueShopping":
                    break;
                case "Update":
                    break;
            }
            return RedirectToAction("Index");
        }

        public ActionResult GetItems()
        {
            var products = _shoppingCart.GetProducts();

            var json = new
            {
                items = (from item in products
                         select new
                         {
                             id = item.ProductPart.Id,
                             sku = item.ProductPart.Sku,
                             type = item.ProductPart.TypeDefinition.Name,
                             title = _services.ContentManager.GetItemMetadata(item.ProductPart).DisplayText ?? "(No TitlePart attached)",
                             unitPrice = item.ProductPart.UnitPrice,
                             quantity = item.Quantity
                         }).ToArray()
            };

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSummary()
        {
            var itemCount = _shoppingCart.ItemCount();
            var total = _shoppingCart.Total();
            var json = new { itemCount = itemCount, total = total };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        private void UpdateShoppingCart(IEnumerable<UpdateShoppingCartItemViewModel> items)
        {

            _shoppingCart.Clear();

            if (items == null)
                return;

            _shoppingCart.AddRange(items
                .Where(item => !item.IsRemoved)
                .Select(item => new ShoppingCartItem(item.ProductId, item.Quantity < 0 ? 0 : item.Quantity))
            );

            _shoppingCart.UpdateItems();
        }
    }
}