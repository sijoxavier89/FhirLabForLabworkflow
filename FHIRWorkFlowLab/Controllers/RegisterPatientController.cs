using FHIRWorkFlowLab.Models.LabRequest;
using FHIRWorkFlowLab.Models.RegisterPatient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FHIRWorkFlowLab.Controllers
{
    public class RegisterPatientController : Controller
    {
        [HttpGet]
        public ActionResult PatientRegister()
        {
            return View();
        }

        [HttpPost]
        public RedirectToRouteResult GeneratePatientResource(RegisterPatientViewModel model)
        {
            LabService lab = new LabService();
            lab.CreatePatientResource(model);
            return RedirectToAction("SearchPatient", "SearchPatient");
        }

    }
}