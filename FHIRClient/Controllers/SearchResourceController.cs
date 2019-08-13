using FHIRClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FHIRClient.Controllers
{
    public class SearchResourceController : Controller
    {
        private readonly ISearchResourceDataService _serviceFactory;

        public SearchResourceController()
        {
            _serviceFactory = new SearchResourceDataService();
        }

        // GET: SearchResource
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetResource(Models.SearchResourceViewModel model)
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