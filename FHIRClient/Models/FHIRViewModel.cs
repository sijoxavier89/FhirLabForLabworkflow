using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using System.ComponentModel.DataAnnotations;

namespace FHIRClient.Models
{
    public class FHIRViewModel
    {
        [Required]
        public string patientId { get; set; }

        public string patientDataJson { get; set; }

        public string ResourceRawJsonData { get; set; }

        public string patientEncounterBundle { get; set; }

        public string patientMedBundle { get; set; }

        public string patientConditionsBundle { get; set; }

        public string patientObservationBundle { get; set; }

        [Required]
        public string resourceInfo { get; set; } 

        public Patient patient { get; set; }

        public List<Encounter> encounters { get; set; }

        public List<MedicationRequest> medicationOrders { get; set; }

        public List<Condition> conditions { get; set; }

        public List<Observation> observations { get; set; }
    }
}