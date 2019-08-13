using Hl7.Fhir.Model;

namespace FHIRClient.Models
{
    public class CreateResourceViewModel
    {
        public Patient myPatient { get; set; }

        public HumanName patientName { get; set; }

        public Identifier patientIdentifier { get; set; }

        public Patient ReturnedPatient { get; set; }

        public string PatientJsonDataSent { get; set; }

        public string PatientJsonDataRecieved { get; set; }

        public string patientPrefix { get; set; }

        public string patientFirstname { get; set; }

        public string patientFamilyName { get; set; }

        public string patientGender { get; set; }

        public string patientDateOfBirth { get; set; }

    }
}