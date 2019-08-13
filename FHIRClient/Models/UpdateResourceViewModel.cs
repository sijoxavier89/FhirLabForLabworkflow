using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHIRClient.Models
{
    public class UpdateResourceViewModel
    {
        public string patientID { get; set; }

        public string patientFirstnameUpdate { get; set; }

        public string patientFamilyNameUpdate { get; set; }

        public string patientGenderUpdate { get; set; }

        public string patientDateOfBirthUpdate { get; set; }

        public HumanName patientFullnameUpdate { get; set; }

        public Patient patientRead { get; set; }

        public Patient patientupdate { get; set; }

        public string patientDetailsOld { get; set; }

        public string patientDetailsUpdated { get; set; }
    }
}