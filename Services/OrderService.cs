using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Data;
using Cascade.WebShop.Models;
using Cascade.WebShop.Extensibility;
using Orchard.Settings;
using Orchard.Services;
using Orchard;
using Cascade.WebShop.Helpers;

namespace Cascade.WebShop.Services
{
    public interface IOrderService : IDependency
    {
        /// <summary>
        /// Creates a new order based on the specified ShoppingCartItems
        /// </summary>
        OrderRecordPart CreateOrder(int customerId, IEnumerable<ShoppingCartItem> items);

        /// <summary>
        /// Creates a new empty order with a status of 'Invalid'
        /// </summary>
        OrderRecordPart CreateOrder();

        /// <summary>
        /// Gets a list of ProductParts from the specified list of OrderDetails. 
        /// Useful if you need to use the product as a ProductPart 
        /// (instead of just having access to the ProductRecord instance).
        /// </summary>
        IEnumerable<ProductPart> GetProducts(IEnumerable<OrderDetail> orderDetails);

        OrderRecordPart GetOrderByNumber(string orderNumber);
        OrderRecordPart GetOrder(int id);

        IContentQuery<OrderRecordPart> GetOrders(int customerId);
        IContentQuery<OrderRecordPart> GetOrders();

        void UpdateOrderStatus(OrderRecordPart order, PaymentResponse paymentResponse);

    }
    
    public class OrderService : IOrderService
    {
        private readonly IRepository<ProductRecord> _productRepository;
        private readonly IContentManager _contentManager;
        private readonly IRepository<OrderRecord> _orderRepository;
        private readonly IRepository<CustomerRecord> _customerRepository;
        private readonly ISiteService _siteService;
        private readonly WebShopSettingsPart _webShopSettings;
        private readonly IClock _clock;

        public OrderService(IRepository<ProductRecord> productRepository, IContentManager contentManager, IRepository<OrderRecord> orderRepository,  IRepository<CustomerRecord> customerRepository, ISiteService siteService, IClock clock)
        {
            _productRepository = productRepository;
            _contentManager = contentManager;
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _siteService = siteService;
            _webShopSettings = _siteService.GetSiteSettings().As<WebShopSettingsPart>();
            _clock = clock;
        }

        public OrderRecordPart CreateOrder()
        {
            var orderPart = _contentManager.Create<OrderRecordPart>("Order");
            orderPart.CreatedAt = _clock.UtcNow.ToLocalTime();
            orderPart.Status = OrderStatus.Invalid;
            orderPart.CustomerId = 0;
            return orderPart;
        }

        public OrderRecordPart CreateOrder(int customerId, IEnumerable<ShoppingCartItem> items)
        {

            if (items == null)
                throw new ArgumentNullException("items");

            // Convert to an array to avoid re-running the enumerable
            var itemsArray = items.ToArray();

            if (!itemsArray.Any())
                throw new ArgumentException("Creating an order with 0 items is not supported", "items");

            var orderPart = CreateOrder();
            orderPart.CustomerId = customerId;
            orderPart.Status = OrderStatus.New;
        
            // Create an order detail for each item
            foreach (var item in itemsArray)
            {
                // This is not very fast but it is flexible and gathers the info we need
                // NOTE the use of a dynamic type below so we don't have to take a dependency on the 
                // external Cascade.ArtStock module for the ArtworkPart.

                var product = _contentManager.Get<ProductPart>(item.ProductId);
                var description = _contentManager.GetItemMetadata(product).DisplayText;
                if (product.ContentItem.Parts.Any(p => p.TypeDefinition.Name == "Artwork"))
                {
                    dynamic part = product;
                    description += " (" + part.ArtworkPart.ArtistRecord.Name + ")";
                }
                else
                    description += " (" + product.ContentItem.TypeDefinition.Name + ")";

                var detail = new OrderDetail
                {
                    OrderRecord_Id = orderPart.Id,
                    ProductPartRecord_Id = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.UnitPrice,
                    GSTRate = .1m,
                    Sku = product.Sku,
                    Description = description
                };

                orderPart.Details.Add(detail);
            }

            orderPart.UpdateTotals();

            return orderPart;
        }

        /// <summary>
        /// Gets a list of ProductParts from the specified list of OrderDetails. Useful if you need to use the product as a ProductPart (instead of just having access to the ProductRecord instance).
        /// </summary>
        public IEnumerable<ProductPart> GetProducts(IEnumerable<OrderDetail> orderDetails)
        {
            var productIds = orderDetails.Select(x => x.ProductPartRecord_Id).ToArray();
            return _contentManager.GetMany<ProductPart>(productIds, VersionOptions.Latest, QueryHints.Empty);
        }

        public OrderRecordPart GetOrderByNumber(string orderNumber)
        {
            var orderId = int.Parse(orderNumber) - 1000;
            return _contentManager.Get<OrderRecordPart>(orderId);
        }

        public OrderRecordPart GetOrder(int id)
        {
            return _contentManager.Get<OrderRecordPart>(id);
        }

        public IContentQuery<OrderRecordPart> GetOrders(int customerId)
        {
            return _contentManager.Query<OrderRecordPart, OrderRecord>().Where(o => o.CustomerId == customerId);
        }

        public IContentQuery<OrderRecordPart> GetOrders()
        {
            return _contentManager.Query<OrderRecordPart, OrderRecord>();
        }


        public void UpdateOrderStatus(OrderRecordPart order, PaymentResponse paymentResponse)
        {
            OrderStatus orderStatus;

            switch (paymentResponse.Status)
            {
                case PaymentResponseStatus.Success:
                    orderStatus = OrderStatus.Paid;
                    break;
                default:
                    orderStatus = OrderStatus.Cancelled;
                    break;
            }

            if (order.Status == orderStatus)
                return;

            order.Status = orderStatus;
            order.PaymentServiceProviderResponse = paymentResponse.ResponseText;
            order.PaymentReference = paymentResponse.PaymentReference;

            switch (order.Status)
            {
                case OrderStatus.Paid:
                    order.PaidAt = DateTime.Now;
                    break;
                case OrderStatus.Completed:
                    order.CompletedAt = DateTime.Now;
                    break;
                case OrderStatus.Cancelled:
                    order.CancelledAt = DateTime.Now;
                    break;
            }
        }

    }
}
