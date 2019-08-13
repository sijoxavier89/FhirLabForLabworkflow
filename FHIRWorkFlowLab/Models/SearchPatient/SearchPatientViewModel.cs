using Hl7.Fhir.Model;

namespace FHIRWorkFlowLab.Models.SearchPatient
{
    public class SearchPatientViewModel
    {
        public string searchPatientMRN { get; set; }

        public Patient patient { get; set; }

        public string ResourceRawJsonData { get; set; }

        public string LabResultRawJsonData { get; set; }
        public string LabRequestRawJsonData { get; set; }
    }
}