using System.Collections.Generic;
using System.Linq;
using Cascade.WebShop.Extensibility;
using Cascade.WebShop.Models;
using Orchard;

namespace Cascade.WebShop.Services
{
    public interface IOrderService : IDependency
    {
        /// <summary>
        /// Creates a new order based on the specified ShoppingCartItems
        /// </summary>
        OrderRecord CreateOrder(int customerId, IEnumerable<ShoppingCartItem> items);

        /// <summary>
        /// Gets a list of ProductParts from the specified list of OrderDetails. Useful if you need to use the product as a ProductPart (instead of just having access to the ProductRecord instance).
        /// </summary>
        IEnumerable<ProductPart> GetProducts(IEnumerable<OrderDetailRecord> orderDetails);

        OrderRecord GetOrderByNumber(string orderNumber);
        
        void UpdateOrderStatus(OrderRecord order, PaymentResponse paymentResponse);

        OrderRecord GetOrder(int id);
        IQueryable<OrderRecord> GetOrders(int customerId);
        IQueryable<OrderRecord> GetOrders();

    }
}