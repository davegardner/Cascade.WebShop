using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Data;
using Cascade.WebShop.Models;
using Cascade.WebShop.Extensibility;
using Orchard.Settings;

namespace Cascade.WebShop.Services
{
    public class OrderService : IOrderService
    {
        //private readonly IDateTimeService _dateTimeService;
        private readonly IRepository<ProductRecord> _productRepository;
        private readonly IContentManager _contentManager;
        private readonly IRepository<OrderRecord> _orderRepository;
        private readonly IRepository<OrderDetailRecord> _orderDetailRepository;
        private readonly IRepository<CustomerRecord> _customerRepository;
        private readonly ISiteService _siteService;
        private readonly WebShopSettingsPart _webShopSettings;

        public OrderService(/*IDateTimeService dateTimeService,*/ IRepository<ProductRecord> productRepository, IContentManager contentManager, IRepository<OrderRecord> orderRepository, IRepository<OrderDetailRecord> orderDetailRepository, IRepository<CustomerRecord> customerRepository, ISiteService siteService)
        {
            //_dateTimeService = dateTimeService;
            _productRepository = productRepository;
            _contentManager = contentManager;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _customerRepository = customerRepository;
            _siteService = siteService;
            _webShopSettings = _siteService.GetSiteSettings().As<WebShopSettingsPart>();
        }

        public OrderRecord CreateOrder(int customerId, IEnumerable<ShoppingCartItem> items)
        {

            if (items == null)
                throw new ArgumentNullException("items");

            // Convert to an array to avoid re-running the enumerable
            var itemsArray = items.ToArray();

            if (!itemsArray.Any())
                throw new ArgumentException("Creating an order with 0 items is not supported", "items");

            var order = new OrderRecord
            {
                //CreatedAt = _dateTimeService.Now,
                CreatedAt = DateTime.Now,
                CustomerId = customerId,
                Status = OrderStatus.New
            };

            _orderRepository.Create(order);

            // add the shipping charges (if any)
            //var shippingProductPart = _contentManager.Get<ShippingProductPart>(_webShopSettings.ShippingProductRecord.Id);
            //if (shippingProductPart != null)
            //{
            //    var shippingProduct = shippingProductPart.As<ProductPart>();
            //    if (shippingProductPart != null)
            //    {
            //        var detail = new OrderDetailRecord
            //        {
            //            OrderRecord_Id = order.Id,
            //            ProductPartRecord_Id = shippingProduct.Record.Id,
            //            Quantity = 1,
            //            UnitPrice = shippingProduct.UnitPrice,
            //            GSTRate = .1m
            //        };
            //        _orderDetailRepository.Create(detail);
            //        order.Details.Add(detail);
            //    }
            //}

            // Get all products in one shot, so we can add the product reference to each order detail
            //var productIds = itemsArray.Select(x => x.ProductId).ToArray();
            //var products = _productRepository.Fetch(x => productIds.Contains(x.Id)).ToArray();

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
                    dynamic part = product ;
                    description += " (" + part.ArtworkPart.ArtistRecord.Name + ")";
                }
                else
                    description += " (" + product.ContentItem.TypeDefinition.Name + ")";

                var detail = new OrderDetailRecord
                {
                    OrderRecord_Id = order.Id,
                    ProductPartRecord_Id = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.UnitPrice,
                    GSTRate = .1m,
                    Sku = product.Sku,
                    Description = description
                };

                _orderDetailRepository.Create(detail);
                order.Details.Add(detail);
            }

            order.UpdateTotals();

            return order;
        }

        /// <summary>
        /// Gets a list of ProductParts from the specified list of OrderDetails. Useful if you need to use the product as a ProductPart (instead of just having access to the ProductRecord instance).
        /// </summary>
        public IEnumerable<ProductPart> GetProducts(IEnumerable<OrderDetailRecord> orderDetails)
        {
            var productIds = orderDetails.Select(x => x.ProductPartRecord_Id).ToArray();
            return _contentManager.GetMany<ProductPart>(productIds, VersionOptions.Latest, QueryHints.Empty);
        }

        public OrderRecord GetOrderByNumber(string orderNumber)
        {
            var orderId = int.Parse(orderNumber) - 1000;
            return _orderRepository.Get(orderId);
        }

        public OrderRecord GetOrder(int id)
        {
            return _orderRepository.Get(id);
        }

        public IQueryable<OrderRecord> GetOrders(int customerId)
        {
            return _orderRepository.Table.Where(r => r.CustomerId == customerId);
        }
        
        public IQueryable<OrderRecord> GetOrders()
        {
            return _orderRepository.Table;
        }

        public void UpdateOrderStatus(OrderRecord order, PaymentResponse paymentResponse)
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
