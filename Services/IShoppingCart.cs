using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Cascade.WebShop.Models;


namespace Cascade.WebShop.Services
{
    public interface IShoppingCart : IDependency
    {
        IEnumerable<ShoppingCartItem> Items { get; }
        void Add(int productId, int quantity = 1);
        void Remove(int productId);
        ProductPart GetProduct(int productId);
        IEnumerable<ProductQuantity> GetProducts();
        decimal Subtotal();
        decimal GST();
        decimal Total();
        int ItemCount();
        void UpdateItems();
        void Clear();
        void AddRange(IEnumerable<ShoppingCartItem> iEnumerable);
        void RemoveFromStock();
    }
}