using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHIRWorkFlowLab.Models.RegisterPatient
{
    public class RegisterPatientViewModel
    {
        public string PatientMRN { get; set; }
        public string Prefix { get; set; }
        public string PatientFirstname { get; set; }
        public string Familyname { get; set; }
        public string gender { get; set; }
        public string DateOfBirth { get; set; }
        public string PatientIdentifier { get; set; }
        public string TelecomDetails { get; set; }
        public string MaritalStatus { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public HumanName PatientName { get; set; }

    }
}