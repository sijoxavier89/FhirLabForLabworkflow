using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;

namespace FHIRClient.Models
{
    public class FHIRClientDataService : IFHIRClientDataService
    {
        private static string OpenFHIRUrl = ConfigurationManager.AppSettings["OpenFHIRUrl"];
        private static FhirJsonParser fhirJsonParser = new FhirJsonParser();
        private string patientId;
        private string resouceInfo;

        public FHIRViewModel GetResourceDetails(string patientId, string resouceInfo)
        {
            this.patientId = patientId;
            this.resouceInfo = resouceInfo;
            FHIRViewModel vm = new FHIRViewModel();

            switch (resouceInfo)
            {
                case "PatientInfo":
                    //Patient Details
                    vm.patientDataJson = GetData(OpenFHIRUrl + "/Patient/" + patientId);
                    vm.patient = (Patient)fhirJsonParser.Parse(vm.patientDataJson, typeof(Patient));
                    vm.ResourceRawJsonData = JValue.Parse(vm.patientDataJson).ToString();
                    break;

                case "EncounterInfo":
                    //Patient Encounter Details
                    vm.patientEncounterBundle = GetData(OpenFHIRUrl + "/Encounter?patient=" + patientId);
                    Bundle encounterBundle = (Bundle)fhirJsonParser.Parse(vm.patientEncounterBundle, typeof(Bundle));
                    vm.encounters = encounterBundle.Entry.Select(Resource => (Encounter)Resource.Resource).ToList();
                    vm.ResourceRawJsonData = JValue.Parse(vm.patientEncounterBundle).ToString();
                    break;

                case "MedicationInfo":
                    //Patient Medication Details
                    vm.patientMedBundle = GetData(OpenFHIRUrl + "/MedicationRequest?patient=" + patientId + "&status=active");
                    Bundle medicationOrderBundle = (Bundle)fhirJsonParser.Parse(vm.patientMedBundle, typeof(Bundle));
                    vm.medicationOrders = medicationOrderBundle.Entry.Select(Resource => (MedicationRequest)Resource.Resource).ToList();
                    vm.ResourceRawJsonData = JValue.Parse(vm.patientMedBundle).ToString();
                    break;

                case "ConditionInfo":
                    //Patient condition Details
                    vm.patientConditionsBundle = GetData(OpenFHIRUrl + "/Condition?patient=" + patientId);
                    Bundle conditionBundle = (Bundle)fhirJsonParser.Parse(vm.patientConditionsBundle, typeof(Bundle));
                    vm.conditions = conditionBundle.Entry.Select(Resource => (Condition)Resource.Resource).ToList();
                    vm.ResourceRawJsonData = JValue.Parse(vm.patientConditionsBundle).ToString();
                    break;

                case "ObservationInfo":
                    //Patient observation Details
                    vm.patientObservationBundle = GetData(OpenFHIRUrl + "/Observation?patient=" + patientId);
                    Bundle observationBundle = (Bundle)fhirJsonParser.Parse(vm.patientObservationBundle, typeof(Bundle));
                    vm.observations = observationBundle.Entry.Select(Resource => (Observation)Resource.Resource).ToList();
                    vm.ResourceRawJsonData = JValue.Parse(vm.patientObservationBundle).ToString();
                    break;              
            }

            return vm;
        }


        private static string GetData(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "GET";

            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string response = streamReader.ReadToEnd();
                return response;
            }
        }
    }
}