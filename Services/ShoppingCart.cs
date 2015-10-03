using Cascade.WebShop.Models;
using Orchard;
using Orchard.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cascade.WebShop.Services
{
    public class ShoppingCart : IShoppingCart
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IContentManager _contentManager;
        private readonly IWebshopSettingsService _webshopSettings;

        public IEnumerable<ShoppingCartItem> Items { get { return ItemsInternal.AsReadOnly(); } }
        bool GSTinc = true; // DG: true if prices include GST

        private HttpContextBase HttpContext
        {
            get { return _workContextAccessor.GetContext().HttpContext; }
        }

        private List<ShoppingCartItem> ItemsInternal
        {
            get
            {
                var items = HttpContext.Session["ShoppingCart"] as List<ShoppingCartItem>;

                if (items == null)
                {
                    items = new List<ShoppingCartItem>();

                    HttpContext.Session["ShoppingCart"] = items;
                }

                return items;
            }
        }

        public ShoppingCart(IWorkContextAccessor workContextAccessor, IContentManager contentManager, IWebshopSettingsService webshopSettings)
        {
            _workContextAccessor = workContextAccessor;
            _contentManager = contentManager;
            _webshopSettings = webshopSettings;
        }

        public void Add(int productId, int quantity = 1)
        {
            // only add if in stock: removed from stock when order processed (
            var product = GetProduct(productId);
            if (product != null && product.InStock > 0)
            {
                var item = Items.SingleOrDefault(x => x.ProductId == productId);

                if (item == null)
                {
                    item = new ShoppingCartItem(productId, quantity);
                    ItemsInternal.Add(item);

                    // add the default shipping 'product' to cover packing and shipping costs
                    var shippingProductId = _webshopSettings.GetShippingProductRecordId();
                    if (shippingProductId > 0 && product.IsShippable && !ItemsInternal.Exists(i => i.ProductId == shippingProductId))
                        ItemsInternal.Add(new ShoppingCartItem(shippingProductId, 1));

                }


                // DG: Comment this out to limit to one item (because it's easy to click 'Add' multiple times)
                // TODO: make it a setting
                else
                {
                    item.Quantity += quantity;
                }
            }
        }

        public void Remove(int productId)
        {
            var item = Items.SingleOrDefault(x => x.ProductId == productId);

            if (item == null)
                return;

            ItemsInternal.Remove(item);
        }

        public ProductPart GetProduct(int productId)
        {
            return _contentManager.Get<ProductPart>(productId);
        }

        public void UpdateItems()
        {
            ItemsInternal.RemoveAll(x => x.Quantity == 0);
        }

        public decimal Subtotal()
        {
            return Items.Select(x => GetProduct(x.ProductId).UnitPrice * x.Quantity).Sum();
        }

        public decimal GST()
        {
            if (GSTinc)
                return Subtotal() / 11m;
            else
                return Subtotal() * 0.1m;
        }

        public decimal Total()
        {
            if (GSTinc)
                return Subtotal();
            else
                return Subtotal() + GST();
        }

        public int ItemCount()
        {
            return Items.Sum(x => x.Quantity);
        }

        public void Clear()
        {
            ItemsInternal.Clear();
            UpdateItems();
        }

        public IEnumerable<ProductQuantity> GetProducts()
        {
            // Get a list of all product IDs from the shopping cart
            var ids = Items.Select(x => x.ProductId).ToList();

            // Load all product parts by the list of IDs
            var productParts = _contentManager.GetMany<ProductPart>(ids, VersionOptions.Latest, QueryHints.Empty).ToArray();

            // Create a LINQ query that projects all items in the shoppingcart into shapes
            // Order by sku is to enable shipping to appear last (if it has lower sku!)
            var query = from item in Items
                        from productPart in productParts
                        where productPart.Id == item.ProductId
                        orderby productPart.Sku descending
                        select new ProductQuantity
                        {
                            ProductPart = productPart,
                            Quantity = item.Quantity
                        };

            return query;
        }

        public void AddRange(IEnumerable<ShoppingCartItem> iEnumerable)
        {
            ItemsInternal.AddRange(iEnumerable);
        }

        public void RemoveFromStock()
        {
            foreach (var item in Items)
            {
                var product = GetProduct(item.ProductId);
                product.InStock -= item.Quantity;
                product.NumberSold += item.Quantity;
            }
        }
    }
}