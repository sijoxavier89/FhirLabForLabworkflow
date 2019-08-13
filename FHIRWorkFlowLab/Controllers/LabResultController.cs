using FHIRWorkFlowLab.Models.LabRequest;
using FHIRWorkFlowLab.Models.LabResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FHIRWorkFlowLab.Controllers
{
    public class LabResultController : Controller
    {
        
        public ActionResult Report(LabResultViewModel modal)
        {
            if (TempData.ContainsKey("LabResult"))
                modal = (LabResultViewModel)TempData["LabResult"];

            return View(modal);
        }

        [HttpPost]
        public ActionResult SendReport(LabResultViewModel modal)
        {

            LabService lab = new LabService();
            lab.SendLabResult(modal);

            return RedirectToAction("List", "LabList");
        }
    }
}