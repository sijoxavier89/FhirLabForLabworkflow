using FHIRWorkFlowLab.Models.SearchPatient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FHIRWorkFlowLab.Controllers
{
    public class SearchPatientController : Controller
    {
        private readonly ISearchPatientDataService _serviceFactory;

        public SearchPatientController()
        {
            _serviceFactory = new SearchPatientDataService();
        }

        [HttpGet]
        public ActionResult SearchPatient()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PatientDetails(SearchPatientViewModel modeldata)
        {
            var patientMRN = string.Empty;

            patientMRN = modeldata.searchPatientMRN;

            var patientResult = _serviceFactory.GetPatientDetails(patientMRN);

            TempData["PatientDetails"] = patientResult.patient;

            if(patientResult.patient != null)
            {
                TempData["PatientName"] = patientResult.patient.Name[0];
            }
            else
            {
                TempData["_Error"] = "No Patient Found";
                return RedirectToAction("Error", "Error");
            }

            return View(patientResult);
        }
    }
}