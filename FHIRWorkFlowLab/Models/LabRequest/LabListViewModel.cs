using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHIRWorkFlowLab.Models.LabRequest
{
    public class LabListViewModel
    {
        public List<LabRequest> LabList { get; set; }
        public string FhirRequest { get; set; }
    }
}