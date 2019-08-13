using FHIRWorkFlowLab.Models.LabRequest;
using FHIRWorkFlowLab.Models.LabResult;
using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FHIRWorkFlowLab.Controllers
{
    public class ViewResultController : Controller
    {
        LabResultViewModel vm = new LabResultViewModel();

        // GET: ViewResult
        public ActionResult ShowResult(string procedureId)
        {
            try
            {
                LabService lab = new LabService();
                var result = lab.GetLabResultByRequestId(procedureId);
                vm.PatientId = result.PatientId;
                vm.Category = result.Category;
                vm.ResultGlucoseLevel = result.ResultGlucoseLevel;
                vm.IssuedDate = result.IssuedDate;
                vm.Status = result.Status;
                vm.FhirResource = result.FhirResource;
            }
            catch (Exception ex)
            {
                if(ex != null)
                {
                    TempData["_Error"] = "Some Error Occurred";
                    return RedirectToAction("Error", "Error");
                }
            }
                        
            return View(vm);
        }
    }
}