using FHIRWorkFlowLab.Models.LabResult;
using FHIRWorkFlowLab.Models.RegisterPatient;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace FHIRWorkFlowLab.Models.LabRequest
{
    public class LabService
    {
        private static string OpenFHIRUrl = ConfigurationManager.AppSettings["OpenFHIRUrl"];
        private static FhirJsonSerializer fhirJsonSerializer = new FhirJsonSerializer();
        private static FhirJsonParser fhirJsonParser = new FhirJsonParser();
        private const string SystemName = "www.citiustech.com";

        private FhirClient FhirClientObj = null;

        public LabService()
        {
             FhirClientObj = new FhirClient(OpenFHIRUrl);
        }

        public void SendLabRequest(LabRequestViewModel labRequest)
        {

            var procedureRequest = CreateProcRequest(labRequest);
            var result= FhirClientObj.Create<ProcedureRequest>(procedureRequest);

        }

        private ProcedureRequest CreateProcRequest(LabRequestViewModel procRequest)
        {
            string identifier = "CT12345";
            string practitionerId = "smart-Practitioner-71081332";
            ProcedureRequest proc = new ProcedureRequest();
            proc.Identifier = new List<Identifier>
            {
                new Identifier() {Value = identifier, System = SystemName}
            };
            proc.Status = RequestStatus.Active;
            proc.Intent = RequestIntent.Order;
            proc.Category = new List<CodeableConcept> { new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding("http://snomed.info/sct",
                    "103693007","Diagnostic procedure" )
                }
            } };

            string measurement = string.Empty;
            string procCode = string.Empty;
            string system = string.Empty;
            if(procRequest.Biochemistry == "Glucose" && procRequest.Sample == "Blood")
            {
                measurement = "Glucose measurement, blood (procedure)";
                procCode = "33474003";
                system = "2.16.840.1.113883.6.96";
            }
            proc.Code = new CodeableConcept(system, procCode, measurement);
            proc.Requester = new ProcedureRequest.RequesterComponent();
            string practitionerRef = "Practitioner/" + practitionerId;
            proc.Requester.Agent = new ResourceReference(practitionerRef);
            string subjectRef = "Patient/" + procRequest.Patient.Id;
            string subjectDisplay = procRequest.Name.ToString();
            proc.Subject = new ResourceReference(subjectRef, subjectDisplay);
            return proc;

        }

        public LabListViewModel GetLabRequests()
        {
            //var searchParams = new SearchParams(Parameters)
            string[] criteria = new string[] { "requester:Practitioner=smart-Practitioner-71081332" };
            var labRequestResponse = FhirClientObj.Search<ProcedureRequest>(criteria);

            return GenerateLabListFromBundle(labRequestResponse);
        }

        public LabListViewModel GetLabRequestsByPatientId(string patientId)
        {
            //var searchParams = new SearchParams(Parameters)
            string[] criteria = new string[] { "subject=Patient/" + patientId };
            var labRequestResponse = FhirClientObj.Search<ProcedureRequest>(criteria);

            return GenerateLabListFromBundle(labRequestResponse);
        }
        private LabListViewModel GenerateLabListFromBundle(Bundle labRequestResponse)
        {
            //Serializing the data to json string
            string labRequestJson = fhirJsonSerializer.SerializeToString(labRequestResponse);

            //Parsing the json string  
            Bundle labRequests = (Bundle)fhirJsonParser.Parse(labRequestJson, typeof(Bundle));

            //Parsing the data to the procedure request list
            List<ProcedureRequest> procedures = new List<ProcedureRequest>();
            procedures = labRequests.Entry.Select(Resource => (ProcedureRequest)Resource.Resource).ToList();
            IEnumerable<ProcedureRequest> ctprocedures = procedures.Where(item => (item.Identifier.Count > 0) && (item.Identifier.FirstOrDefault().System == SystemName));
            List<LabRequest> labRequest = new List<LabRequest>();
            foreach (var item in ctprocedures)
            {
                labRequest.Add(new LabRequest
                {
                    Id = string.IsNullOrEmpty(item.Id) ? string.Empty : item.Id,
                    Category = string.IsNullOrEmpty(item.Category.FirstOrDefault().Coding.FirstOrDefault().Display) ? string.Empty : item.Category.FirstOrDefault().Coding.FirstOrDefault().Display,
                    PractitionerId = string.IsNullOrEmpty(item.Requester.Agent.Reference) ? string.Empty : item.Requester.Agent.Reference,
                    Status = string.IsNullOrEmpty(item.Status.ToString()) ? string.Empty : item.Status.ToString(),
                    PatientID = string.IsNullOrEmpty(item.Subject.Reference) ? string.Empty : item.Subject.Reference,
                    PatientName = string.IsNullOrEmpty(item.Subject.Display) ? string.Empty : item.Subject.Display

                }
               );
            }
            LabListViewModel labList = new LabListViewModel();

            labList.LabList = labRequest;
            labList.FhirRequest = labRequestJson;

            labList.FhirRequest = JValue.Parse(labRequestJson).ToString();

            return labList;
        }
        public void SendLabResult(LabResultViewModel labResult)
        {

            var observation = CreateLabResultResource(labResult);
            var result = FhirClientObj.Create<Observation>(observation);
            UpdateProcedureByProcedureId(labResult.ProcedureId);
        }

        private Observation CreateLabResultResource(LabResultViewModel labResult)
        {
            string identifier = "CTOBS-123";
            Observation observation = new Observation();
            observation.Identifier = new List<Identifier>
            {
                new Identifier() {Value = identifier, System = SystemName }
            };

            string procedureRequestRef = "ProcedureRequest/" + labResult.ProcedureId;
            observation.BasedOn = new List<ResourceReference>()
            {
                new ResourceReference(procedureRequestRef)
            };

            observation.Status = ObservationStatus.Final;

            string observationCode = "15074-8";
            string observationSystem = "http://loinc.org";
            string observationDisplay = "Glucose [Moles/volume] in Blood";

            observation.Code = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding(observationSystem,
                    observationCode,observationDisplay )
                }
            };

            string subjectRef = labResult.PatientId;
            observation.Subject = new ResourceReference(subjectRef);

            string performer = "CTlab";
            string performerRef = "Organization/" + performer;
            observation.Performer = new List<ResourceReference>() { new ResourceReference(performerRef) };

            string obsValueCodeSystem = "http://unitsofmeasure.org";
            decimal obsValue = Convert.ToDecimal(labResult.ResultGlucoseLevel);
            string obsValueUnity = labResult.ResultUnit;
            observation.Value = new Quantity(obsValue, obsValueUnity, obsValueCodeSystem);

            string methodSystem = "http://snomed.info/sct";
            string methodCode = "707692008";
            string methodDisplay = "Corrected for glucose technique";
            observation.Method = new CodeableConcept(methodSystem, methodCode, methodDisplay);

            observation.Issued = Convert.ToDateTime(labResult.IssuedDate);

            return observation;
        }

        public void UpdateProcedureByProcedureId(string Id)
        {
            var labRequest = GetLabRequestById(Id);

            labRequest.Status = RequestStatus.Completed;
            var result = FhirClientObj.Update<ProcedureRequest>(labRequest);
        }

        public ProcedureRequest GetLabRequestById(string procedureId)
        {
            var resourceToUpdate = ("/ProcedureRequest/" + procedureId);

            //Reading the data at the given location("URL") 
            var procedure = FhirClientObj.Read<ProcedureRequest>(resourceToUpdate);

            return procedure;
        }

        /// <summary>
        /// To get lab result details based on procedure id
        /// </summary>
        /// <param name="procedureId"></param>
        /// <returns></returns>
        public LabResultViewModel GetLabResultByRequestId(string procedureId)
        {
            LabResultViewModel labresult = new LabResultViewModel();

            try
            {
                string[] criteria = new string[] { "based-on:ProcedureRequest=" + procedureId };
                var labResultResponse = FhirClientObj.Search<Observation>(criteria);

                //Serializing the data to json string
                string labResultJson = fhirJsonSerializer.SerializeToString(labResultResponse);

                //Parsing the json string  
                Bundle labResults = (Bundle)fhirJsonParser.Parse(labResultJson, typeof(Bundle));

                //Parsing the data to the procedure request list
                Observation observation;
                observation = labResults.Entry.Select(Resource => (Observation)Resource.Resource).ToList().FirstOrDefault();
                               
                labresult.Category = observation.Code.Coding.FirstOrDefault().Display;
                labresult.PatientId = observation.Subject.Reference;
                labresult.ResultGlucoseLevel = ((Quantity)observation.Value).Value.ToString();
                labresult.IssuedDate = observation.Issued.ToString();
                labresult.Status = observation.Status.ToString();
                labresult.FhirResource = JValue.Parse(labResultJson).ToString();
            }
            catch (Exception ex)
            {
                var response = new StreamReader(ex.Message).ReadToEnd();
                var error = JsonConvert.DeserializeObject(response);
                labresult.Error = error.ToString();
            }

            return labresult;
        }

        public void CreatePatientResource(RegisterPatientViewModel patientInfo)
        {
            var patientDetails = new Patient();

            patientInfo.PatientName = new HumanName();
            patientInfo.PatientName.Use = HumanName.NameUse.Official;
            patientInfo.PatientName.Prefix = new string[] { patientInfo.Prefix };
            patientInfo.PatientName.Given = new string[] { patientInfo.PatientFirstname };
            patientInfo.PatientName.Family = patientInfo.Familyname;
            patientDetails.Name = new List<HumanName>();
            patientDetails.Name.Add(patientInfo.PatientName);

            patientDetails.Gender = patientInfo.gender == "M" ? AdministrativeGender.Male : patientInfo.gender == "F" ? AdministrativeGender.Female : AdministrativeGender.Other;
            patientDetails.BirthDate = patientInfo.DateOfBirth;

            patientDetails.Identifier = new List<Identifier>
            {
                new Identifier() {Value = patientInfo.PatientMRN, System = SystemName }
            };

            var maritalStatusSystem = "http://hl7.org/fhir/v3/MaritalStatus";
            var maritalStatusCode = patientInfo.MaritalStatus;
            patientDetails.MaritalStatus = new CodeableConcept
            {
                Text = patientInfo.MaritalStatus,
                Coding = new List<Coding>
                {
                    new Coding
                    (
                         maritalStatusSystem,maritalStatusCode
                    )
                }
            };

            var Use = ContactPoint.ContactPointUse.Home;
            var System = ContactPoint.ContactPointSystem.Phone;
            var Value = patientInfo.TelecomDetails;
            patientDetails.Telecom = new List<ContactPoint>
            {
                new ContactPoint
                (
                  System,Use,Value
                )
            };

            patientDetails.Address = new List<Address>()
            {
                new Address()
                {
                    Line = new string[] { patientInfo.Address} ,
                    City = patientInfo.City ,
                    State = patientInfo.State ,
                    Country = patientInfo.Country,
                    PostalCode = patientInfo.Zip
                }
            };

            var result = FhirClientObj.Create<Patient>(patientDetails);
        }   
        /// <summary>
        /// Get lab request associated with the patient
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public Bundle GetLabRequestByPatientId(string patientId)
        {
            string[] criteria = new string[] { "subject=Patient/" + patientId };
            var labResultResponse = FhirClientObj.Search<ProcedureRequest>(criteria);

            //Serializing the data to json string
            string labResultJson = fhirJsonSerializer.SerializeToString(labResultResponse);

            //Parsing the json string  
            Bundle labResults = (Bundle)fhirJsonParser.Parse(labResultJson, typeof(Bundle));
            return labResults;
        }

        public Bundle GetLabResultsByPatientId(string patientId)
        {
            string[] criteria = new string[] { "subject=Patient/" + patientId };
            var labResultResponse = FhirClientObj.Search<Observation>(criteria);

            //Serializing the data to json string
            string labResultJson = fhirJsonSerializer.SerializeToString(labResultResponse);

            //Parsing the json string  
            Bundle labResults = (Bundle)fhirJsonParser.Parse(labResultJson, typeof(Bundle));
            return labResults;
        }
    }
}