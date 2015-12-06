using System.Linq;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Users.Models;
using Cascade.WebShop.Models;
using Cascade.WebShop.Services;
using Cascade.WebShop.ViewModels;

namespace Cascade.WebShop.Controllers
{
    [Admin]
    public class OrderAdminController : Controller
    {
        private dynamic Shape { get; set; }
        private Localizer T { get; set; }
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IRepository<ProductRecord> _productRepository;
        private readonly INotifier _notifier;
        private readonly ISiteService _siteService;
        private readonly IRepository<TransactionRecord> _transactionRepository;

        public OrderAdminController(INotifier notifier, IOrderService orderService, IShapeFactory shapeFactory, IRepository<ProductRecord> productRepository, ISiteService siteService, IRepository<TransactionRecord> transactionRepository, ICustomerService customerService)
        {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
            _notifier = notifier;
            _orderService = orderService;
            _productRepository = productRepository;
            _siteService = siteService;
            _transactionRepository = transactionRepository;
            _customerService = customerService;
        }

        public ActionResult Index(PagerParameters pagerParameters)
        {
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
            var ordersQuery = _orderService.GetOrders().List().OrderByDescending(o => o.CreatedAt);
            var orders = ordersQuery.Skip(pager.GetStartIndex()).Take(pager.PageSize);
            var pagerShape = Shape.Pager(pager).TotalItemCount(ordersQuery.Count());
            var model = Shape.Orders(Orders: orders.ToArray(), Pager: pagerShape);
            return View((object)model);
        }

        public ActionResult Edit(int id)
        {
            var order = _orderService.GetOrder(id);
            var model = BuildModel(order, new EditOrderVM(order));
            return View((object)model);
        }

        [HttpPost]
        public ActionResult Edit(EditOrderVM model)
        {
            var order = _orderService.GetOrder(model.Id);

            if (!ModelState.IsValid)
                return BuildModel(order, model);

            order.Status = model.Status;

            _notifier.Add(NotifyType.Information, T("The order has been saved"));
            return RedirectToAction("ListOrders", "CustomerAdmin", new { id = order.CustomerId });
        }

        public ActionResult History(int id)
        {
            var history = _transactionRepository.Fetch(t => t.OrderRecord_Id == id);
            var model = Shape.History(History: history);
            return View((object)model);
        }

        private dynamic BuildModel(OrderRecordPart order, EditOrderVM editModel)
        {
            CustomerPart customer = _customerService.GetCustomer(order.CustomerId);
            AddressPart shipAddressPart = _customerService.GetShippingAddress(customer.Id, order.Id);
            AddressPart custAddressPart = _customerService.GetInvoiceAddress(customer.Id);

            string shipName = string.Empty;
            if (!string.IsNullOrWhiteSpace(shipAddressPart.Name))
                shipName = shipAddressPart.Name;

            string shipAddress = shipAddressPart.Address;
            string shipAddress2 = shipAddressPart.City + " " + shipAddressPart.State + " " + shipAddressPart.Postcode;
            string shipCountry = shipAddressPart.Country;

            string custAddress = custAddressPart.Address;
            string custAddress2 = custAddressPart.City + " " + custAddressPart.State + " " + custAddressPart.Postcode;
            string custCountry = custAddressPart.Country;

            return Shape.Order(
                Order: order,
                CustomerName: customer.FullName,
                CustomerAddress1: custAddress,
                CustomerAddress2: custAddress2,
                CustomerCountry: custCountry,
                ShippingName: shipName,
                ShippingAddress1: shipAddress,
                ShippingAddress2: shipAddress2,
                ShippingCountry : shipCountry,
                Details: null, // Convert the whole thing to use Part instead of Record
                //Details: order.Details.Select( detail =>
                //    Shape.Detail
                //    (
                //        Sku: detail.Sku,
                //        Price: detail.UnitPrice,
                //        Quantity: detail.Quantity,
                //        Total: detail.Total,
                //        Description: detail.Description
                //    )).ToArray(),
                EditModel: editModel
            );
        }
    }
}