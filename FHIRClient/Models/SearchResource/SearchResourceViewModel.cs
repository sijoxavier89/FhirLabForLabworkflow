using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHIRClient.Models
{
    public class SearchResourceViewModel
    {
        public string patientId { get; set; }

        public string resourceInfo { get; set; }

        public string ResourceRawJsonData { get; set; }

        public Patient patient { get; set; }

        public List<Encounter> encounters { get; set; }

        public List<MedicationRequest> medicationRequest { get; set; }

        public List<Condition> conditions { get; set; }

        public List<Observation> observations { get; set; }

        public string operationOutcomeError { get; set; }
    }
}