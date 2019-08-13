using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionResource
{
    class Program
    {
        private static string OpenFHIRUrl = ConfigurationManager.AppSettings["OpenFHIRUrl"];

        const string patientId = "1354839";

        public static void Main(string[] args)
        {
            Console.WriteLine("Press any key to create Bundle resource for Transaction.....");
            Console.ReadLine();
            Console.WriteLine("Request : ");

            //creating a patient resource model with hard coded values

            var myPatient = new Patient();
            var patientName = new HumanName();
            patientName.Use = HumanName.NameUse.Official;
            patientName.Prefix = new string[] { "Mr" };
            patientName.Given = new string[] { "John" };
            patientName.Family = "Doe";
            myPatient.Gender = AdministrativeGender.Male;
            myPatient.BirthDate = "1991-05-04";
            myPatient.Name = new List<HumanName>();
            myPatient.Name.Add(patientName);

            var raceExtension = new Extension();
            raceExtension.Url = "http://hl7api.sourceforge.net/hapi-fhir/res/raceExt.html";
            raceExtension.Value = new Code { Value = "WHITE" };
            myPatient.Extension.Add(raceExtension);

            //Creating a Encounter resource model with hard coded values

            var myEncounter = new Encounter();
            myEncounter.Text = new Narrative() { Status=Narrative.NarrativeStatus.Generated,Div= "<div xmlns=\"http://www.w3.org/1999/xhtml\">Encounter with patient @example</div>" };
            myEncounter.Status = Encounter.EncounterStatus.InProgress;
            myEncounter.Class = new Coding() { System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",Code = "IMP", Display = "inpatient encounter" };


            //Updating a Patient resource considering the patient ID already exist on the server
            //If the patient ID is not present kindly select a patient Id already existing on the server else it will give error since we are hard coding it

            var updatePatient = new Patient(); //1354839
            updatePatient.Id = patientId;
            var patientNameUpdate = new HumanName();
            patientNameUpdate.Use = HumanName.NameUse.Official;
            patientNameUpdate.Prefix = new string[] { "Mr" };
            patientNameUpdate.Given = new string[] { "Smith" };
            patientNameUpdate.Family = "Carl";
            updatePatient.Name = new List<HumanName>();
            updatePatient.Name.Add(patientNameUpdate);

            //creating bundle for the transaction
            var bundle = new Bundle();

            bundle.AddResourceEntry(myPatient, "https://hapi.fhir.org/baseDstu3/Patient").Request = new Bundle.RequestComponent { Method = Bundle.HTTPVerb.POST, Url = "Patient" };
            bundle.AddResourceEntry(myEncounter, "https://hapi.fhir.org/baseDstu3/Encounter").Request = new Bundle.RequestComponent { Method = Bundle.HTTPVerb.POST, Url = "Encounter" };
            bundle.AddResourceEntry(updatePatient, "https://hapi.fhir.org/baseDstu3/Patient/1354839").Request = new Bundle.RequestComponent { Method = Bundle.HTTPVerb.PUT, Url = "Patient/1354839" };

            //Create a client to send to the server at a given endpoint.
            var FhirClient = new Hl7.Fhir.Rest.FhirClient(OpenFHIRUrl);

            string requestData = new FhirJsonSerializer().SerializeToString(bundle);
            Console.Write(JValue.Parse(requestData).ToString());

            //sending the Request to the server
            var response = FhirClient.Transaction(bundle);

            Console.WriteLine();
            Console.WriteLine("Press any key to get the response from the server.....");
            Console.ReadLine();
            Console.WriteLine("Response : ");

            string responseData = new FhirJsonSerializer().SerializeToString(response);
            Console.Write(JValue.Parse(responseData).ToString());

            Console.ReadKey();
        }
    }
}
