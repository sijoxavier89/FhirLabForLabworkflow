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

namespace FHIRClient.Models
{
    public class SearchResourceDataService : ISearchResourceDataService
    {
        private static string OpenFHIRUrl = ConfigurationManager.AppSettings["OpenFHIRUrl"];

        private static FhirJsonParser fhirJsonParser = new FhirJsonParser();
        private static FhirJsonSerializer fhirJsonSerializer = new FhirJsonSerializer();

        SearchParams parms = new SearchParams();

        private static FhirClient FhirClient = new FhirClient(OpenFHIRUrl);

        private string patientId;
        private string resouceInfo;

        public SearchResourceViewModel GetResourceDetails(string patientId, string resouceInfo)
        {
            this.patientId = patientId;
            this.resouceInfo = resouceInfo;
            SearchResourceViewModel vm = new SearchResourceViewModel();

            try
            {
                switch (resouceInfo)
                {
                    //Patient Details
                    case "PatientInfo":

                        //Creating the endpoint url with patient patient parameter
                        var ResourceUrl = ("/Patient/" + patientId);

                        //Reading the Patient details using the FHIR Client
                        var patientDetailsRead = FhirClient.Read<Patient>(ResourceUrl);

                        //Serializing the data to json string
                        string patientDetails = fhirJsonSerializer.SerializeToString(patientDetailsRead);

                        //deserializing the json string to patient model 
                        vm.patient = (Patient)fhirJsonParser.Parse(patientDetails, typeof(Patient));

                        //Displaying the Div element of the text section
                        vm.operationOutcomeError = vm.patient.Text == null ? string.Empty : vm.patient.Text.Div;

                        //Displaying the RAW json data in the view 
                        vm.ResourceRawJsonData = JValue.Parse(patientDetails).ToString();
                        break;

                    //Patient Encounter Details
                    case "EncounterInfo":

                        //Adding the Parameter to the URL to get the Encounter related to specific patient using the SearchParams
                        parms.Add("patient", patientId);

                        //Searching the Encounter for the patient
                        var encounterDetailsRead = FhirClient.Search<Encounter>(parms);

                        //Serializing the data to json string
                        string encounterDetails = fhirJsonSerializer.SerializeToString(encounterDetailsRead);

                        //Parsing the json string  
                        Bundle encounters = (Bundle)fhirJsonParser.Parse(encounterDetails, typeof(Bundle));

                        //Parsing the data to the encounter model
                        vm.encounters = encounters.Entry.Select(Resource => (Encounter)Resource.Resource).ToList();

                        //Displaying the Div element of the text section
                        foreach (var enc in vm.encounters)
                        {
                            vm.operationOutcomeError = enc.Text == null ? string.Empty : enc.Text.Div;
                        }

                        //Displaying the RAW json data in the view 
                        vm.ResourceRawJsonData = JValue.Parse(encounterDetails).ToString();
                        break;

                    //Patient Medication Details
                    case "MedicationRequest":

                        //Adding the Parameter to the URL to get the Medication related to specific patient using the SearchParams
                        var Status = "active";
                        parms.Add("patient", patientId);
                        parms.Add("status", Status);

                        //Searching the Medication Request for the patient
                        var medicationDetailsRead = FhirClient.Search<MedicationRequest>(parms);

                        //Serializing the data to json string
                        string medicationDetails = fhirJsonSerializer.SerializeToString(medicationDetailsRead);

                        //Parsing the json string  
                        Bundle medications = (Bundle)fhirJsonParser.Parse(medicationDetails, typeof(Bundle));

                        //Parsing the data to the medication model
                        vm.medicationRequest = medications.Entry.Select(Resource => (MedicationRequest)Resource.Resource).ToList();

                        //Displaying the Div element of the text section
                        foreach (var medreq in vm.medicationRequest)
                        {
                            vm.operationOutcomeError = medreq.Text == null ? string.Empty : medreq.Text.Div;
                        }

                        //Displaying the RAW json data in the view 
                        vm.ResourceRawJsonData = JValue.Parse(medicationDetails).ToString();
                        break;

                    //Patient condition Details
                    case "ConditionInfo":

                        //Adding the Parameter to the URL to get the Condition related to specific patient using the SearchParams
                        parms.Add("patient", patientId);

                        //Searching the Conditons for the patient
                        var conditionDetailsRead = FhirClient.Search<Condition>(parms);

                        //Serializing the data to json string
                        string conditionDetails = fhirJsonSerializer.SerializeToString(conditionDetailsRead);

                        //Parsing the json string  
                        Bundle conditions = (Bundle)fhirJsonParser.Parse(conditionDetails, typeof(Bundle));

                        //Parsing the data to the condition model
                        vm.conditions = conditions.Entry.Select(Resource => (Condition)Resource.Resource).ToList();

                        //Displaying the Div element of the text section
                        foreach (var con in vm.conditions)
                        {
                            vm.operationOutcomeError = con.Text == null ? string.Empty : con.Text.Div;
                        }

                        //Displaying the RAW json data in the view 
                        vm.ResourceRawJsonData = JValue.Parse(conditionDetails).ToString();
                        break;

                    //Patient observation Details
                    case "ObservationInfo":

                        //Adding the Parameter to the URL to get the Observation related to specific patient using the SearchParams
                        parms.Add("patient", patientId);

                        //Searching the Observation Request for the patient
                        var observationDetailsRead = FhirClient.Search<Observation>(parms);

                        //Serializing the data to json string
                        string observationDetails = fhirJsonSerializer.SerializeToString(observationDetailsRead);

                        //Parsing the json string  
                        Bundle observations = (Bundle)fhirJsonParser.Parse(observationDetails, typeof(Bundle));

                        //Parsing the data to the Observation model
                        vm.observations = observations.Entry.Select(Resource => (Observation)Resource.Resource).ToList();

                        //Displaying the Div element of the text section
                        foreach (var obs in vm.observations)
                        {
                            vm.operationOutcomeError = obs.Text == null ? string.Empty : obs.Text.Div;
                        }

                        //Displaying the RAW json data in the view 
                        vm.ResourceRawJsonData = JValue.Parse(observationDetails).ToString();
                        break;
                }
            }

            catch (FhirOperationException FhirOpExec)
            {
                var response = FhirOpExec.Outcome;
                var errorDetails = fhirJsonSerializer.SerializeToString(response);
                OperationOutcome error = (OperationOutcome)fhirJsonParser.Parse(errorDetails, typeof(OperationOutcome));
                vm.operationOutcomeError = error.Text == null ? string.Empty : error.Text.Div;
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