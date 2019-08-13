using FHIRClient.Models;
using System.Web.Mvc;

namespace FHIRClient.Controllers
{
    /// <summary>
    /// IT WILL PERFORM THE SEARCH FUNCTIONALITY USING THE HTTP WEB REQUEST
    /// </summary>
    public class FindResourceController : Controller
    {
        private readonly IFHIRClientDataService _serviceFactory;

        public FindResourceController()
        {
            _serviceFactory = new FHIRClientDataService();
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