using FHIRWorkFlowLab.Models.LabRequest;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;


namespace FHIRWorkFlowLab.Models.SearchPatient
{
    public class SearchPatientDataService : ISearchPatientDataService
    {
        private static string OpenFHIRUrl = ConfigurationManager.AppSettings["OpenFHIRUrl"];

        private static FhirJsonParser fhirJsonParser = new FhirJsonParser();
        private static FhirJsonSerializer fhirJsonSerializer = new FhirJsonSerializer();

        private static FhirClient FhirClient = new FhirClient(OpenFHIRUrl);

        private string patientMRN;

        public SearchPatientViewModel GetPatientDetails(string patientMRN)
        {
            //this.patientMRN = patientId;
            SearchPatientViewModel vm = new SearchPatientViewModel();

            try
            {
                SearchParams parms = new SearchParams();
                var resourceLink = string.Format("{0}|{1}", "www.citiustech.com", patientMRN);
                parms.Add("identifier", resourceLink);

                //var ResourceUrl = ("/Patient?identifier=www.citiustech.com|" + this.patientMRN);

                //Reading the Patient details using the FHIR Client
                var patientDetailsRead = FhirClient.Search<Patient>(parms);

                //Serializing the data to json string
                string patientDetails = fhirJsonSerializer.SerializeToString(patientDetailsRead);

                //deserializing the json string to patient model 
                Bundle result = (Bundle)fhirJsonParser.Parse(patientDetails, typeof(Bundle));

                vm.patient = result.Entry.Select(Resource => (Patient)Resource.Resource).ToList().FirstOrDefault();

                LabService labService = new LabService();
                var labResults = labService.GetLabResultsByPatientId(vm.patient.Id);
                var labRequests = labService.GetLabResultsByPatientId(vm.patient.Id);

                //Displaying the RAW json data in the view 
                vm.ResourceRawJsonData = JValue.Parse(patientDetails).ToString();

                vm.LabRequestRawJsonData = JValue.Parse(fhirJsonSerializer.SerializeToString(labResults)).ToString();

                vm.LabResultRawJsonData = JValue.Parse(fhirJsonSerializer.SerializeToString(labRequests)).ToString();
            }
            catch (FhirOperationException FhirOpExec)
            {
                var response = FhirOpExec.Outcome;
                var errorDetails = fhirJsonSerializer.SerializeToString(response);
                OperationOutcome error = (OperationOutcome)fhirJsonParser.Parse(errorDetails, typeof(OperationOutcome));
                vm.ResourceRawJsonData = JValue.Parse(errorDetails).ToString();
            }
            catch (WebException ex)
            {
                var response = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                var error = JsonConvert.DeserializeObject(response);
                vm.ResourceRawJsonData = error.ToString();
            }

            return vm;
        }
    }
}