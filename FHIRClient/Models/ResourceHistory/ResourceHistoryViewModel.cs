using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHIRClient.Models.ResourceHistory
{
    public class ResourceHistoryViewModel
    {
        public string patientID { get; set; }

        public List<Patient> OperationOutcomePatient { get; set; }
    }
}