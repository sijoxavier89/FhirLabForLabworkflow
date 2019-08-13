using FHIRClient.Models;
using System.Web.Mvc;

namespace FHIRClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFHIRClientDataService _serviceFactory;

        public HomeController()
        {
            _serviceFactory = new FHIRClientDataService();
        }

        public HomeController(IFHIRClientDataService serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PatientDetails(FHIRViewModel model)
        {
            var patientId = string.Empty;
            var resouceInfo = string.Empty;
            patientId = model.patientId;
            resouceInfo = model.resourceInfo;

            var result = _serviceFactory.GetResourceDetails(patientId, resouceInfo);
            return View(result);
        }
    }
}