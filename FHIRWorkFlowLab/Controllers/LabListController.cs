using FHIRWorkFlowLab.Models.LabRequest;
using FHIRWorkFlowLab.Models.LabResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FHIRWorkFlowLab.Controllers
{
    public class LabListController : Controller
    {
        // GET: LabList
        public ActionResult List()
        {
            LabService lab = new LabService();
            var labList = lab.GetLabRequests();
            return View(labList);
        }

        public ActionResult ListByPatient(string patientId)
        {
            LabService lab = new LabService();
            var labList = lab.GetLabRequestsByPatientId(patientId);
            return View("List",labList);
        }
        public RedirectToRouteResult Labtest(string procedureId,string status,string patientId,string practitionerId,string patientName)
        {
            LabResultViewModel labresult = new LabResultViewModel();
            labresult.ProcedureId = procedureId;
            labresult.Status = status;
            labresult.PatientId = patientId;
            labresult.Performer = practitionerId;
            labresult.Category = "Glucose measurement, blood (procedure)";
            labresult.PatientName = patientName;
            TempData["LabResult"] = labresult;

            return RedirectToAction("Report", "LabResult");
        }
    }
}