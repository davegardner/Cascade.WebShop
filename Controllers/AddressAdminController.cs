using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Cascade.WebShop.Services;

namespace Orchard.Webshop.Controllers
{
    [Admin]
    public class AddressAdminController : Controller, IUpdateModel
    {
        private dynamic Shape { get; set; }
        private Localizer T { get; set; }
        private readonly ICustomerService _customerService;
        private readonly IContentManager _contentManager;
        private readonly INotifier _notifier;

        public AddressAdminController
        (
            ICustomerService customerService,
            IShapeFactory shapeFactory,
            IContentManager contentManager,
            INotifier notifier
        )
        {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
            _customerService = customerService;
            _contentManager = contentManager;
            _notifier = notifier;
        }

        public ActionResult Edit(int id)
        {
            var address = _customerService.GetAddress(id);
            var model = _contentManager.BuildEditor(address);

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)model);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(int id)
        {
            var address = _customerService.GetAddress(id);
            var model = _contentManager.UpdateEditor(address, this);

            if (!ModelState.IsValid)
                return View(model);

            _notifier.Add(NotifyType.Information, T("Your address has been saved"));
            return RedirectToAction("ListAddresses", "CustomerAdmin", new { customerId = address.CustomerId });
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.Text);
        }
    }
}