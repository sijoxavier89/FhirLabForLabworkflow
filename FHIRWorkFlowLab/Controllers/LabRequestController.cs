using FHIRWorkFlowLab.Models.LabRequest;
using FHIRWorkFlowLab.Models.SearchPatient;
using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FHIRWorkFlowLab.Controllers
{
    public class LabRequestController : Controller
    {
        // GET: LabRequest
        public ActionResult GenerateLabRequest(SearchPatientViewModel model)
        {
            LabRequestViewModel labRequest = new LabRequestViewModel();

            if (TempData.ContainsKey("PatientDetails"))
            {
                model.patient = (Patient)TempData["PatientDetails"];
                labRequest.Patient = model.patient;
                labRequest.Sample = "Blood";
            }
            else
            {
                TempData["_Error"] = "Some Error Occurred";
                return RedirectToAction("Error", "Error");
            }
                      
            return View(labRequest);
        }

        [HttpPost]
        public RedirectToRouteResult Create(LabRequestViewModel model)
        {
            if (TempData.ContainsKey("PatientName"))
            {
                model.Name = (HumanName)TempData["PatientName"];
            }
            string sample = model.Sample;
            LabService lab = new LabService();
            lab.SendLabRequest(model);
            return  RedirectToAction("List", "LabList");
        }
    }
}