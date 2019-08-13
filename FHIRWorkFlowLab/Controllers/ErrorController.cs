using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FHIRWorkFlowLab.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Error()
        {
            string error = string.Empty;

            Models.Error.ErrorViewModel vm = new Models.Error.ErrorViewModel();

            if (TempData.ContainsKey("_Error"))
            {
                vm.Errormessage = (string)TempData["_Error"];
            }

            return View(vm);
        }
    }
}