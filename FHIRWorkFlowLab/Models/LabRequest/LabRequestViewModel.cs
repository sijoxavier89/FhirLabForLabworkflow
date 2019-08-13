using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHIRWorkFlowLab.Models.LabRequest
{
    public class LabRequestViewModel
    {
        public string PatientId { get; set; }
        public HumanName Name { get; set; }
        public string Gender { get; set; }
        public string Dob { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Sample { get; set; }
        public string Biochemistry { get; set; }
        public Patient Patient { get; set; }
    }
}