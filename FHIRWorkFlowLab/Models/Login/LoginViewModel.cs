using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHIRWorkFlowLab.Models.Login
{
    public class LoginViewModel
    {
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }
    }
}