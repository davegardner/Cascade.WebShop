using System.Web.Mvc;
using Orchard;

namespace Cascade.WebShop.Controllers
{
    public class ResourceController : Controller
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public ResourceController(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        public string SetCultureScript()
        {
            return string.Format("Globalize.culture(\"{0}\");", _workContextAccessor.GetContext().CurrentCulture);
        }
    }
}