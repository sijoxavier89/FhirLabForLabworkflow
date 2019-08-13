using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHIRWorkFlowLab.Models.LabResult
{
    public class LabResultViewModel
    {
        public string PatientName { get; set; }
        public string PatientId { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public string Performer { get; set; }
        public string ResultGlucoseLevel { get; set; }
        public string ResultUnit { get; set; }
        public string IssuedDate { get; set; }
        public string Interpretation { get; set; }
        public string ProcedureId { get; set; }
        public string FhirResource { get; set; }
        public string Role { get; set; }
        public string Error { get; set; }
    }
}