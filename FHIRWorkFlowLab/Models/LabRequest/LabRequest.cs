using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHIRWorkFlowLab.Models.LabRequest
{
    public class LabRequest
    {
        public string Id { get; set; }
        public string Category { get; set; }
        public string PractitionerId { get; set; }
        public string Status { get; set; }
        public string PatientID { get; set; }
        public string PatientName { get; set; }
    }
}