using FHIRWorkFlowLab.Models.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FHIRWorkFlowLab.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            Session["login"] = false;
            return View();
        }

        [HttpPost]
        public ActionResult redirectToPage(LoginViewModel log)
        {
            var email = log.email;
            var password = log.password;
            var role = log.role;
            Session["login"] = false;
           
            if (email == "practitioner@sipraclinic.com" && password == "practitioner" && role == "practitioner")
            {
                Session["login"] = true;
                Session["role"] = role;
                return RedirectToAction("SearchPatient", "SearchPatient");
            }
            else if(email == "labtechnician@sipraclinic.com" && password == "labtechnician" && role == "labTechnician")
            {
                Session["login"] = true;
                Session["role"] = role;
                return RedirectToAction("List", "LabList");
            }
            else
            {
                TempData["_Error"] = "Invalid Credentials";
                return RedirectToAction("Error", "Error");
            }        
        }

    }
}