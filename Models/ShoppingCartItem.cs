using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cascade.WebShop.Models
{
    [Serializable]
    public sealed class ShoppingCartItem {
        public int ProductId { get; private set; }
 
        private int _quantity;
        public int Quantity {
            get { return _quantity; }
            set {
                if (value < 0)
                    throw new IndexOutOfRangeException();
 
                _quantity = value;
            }
        }
 
        public ShoppingCartItem() {
        }
 
        public ShoppingCartItem(int productId, int quantity = 1) {
            ProductId = productId;
            Quantity = quantity;
        }
    }
 }