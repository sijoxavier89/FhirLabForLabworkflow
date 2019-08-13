using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Mvc;

namespace FHIRClient.Controllers
{
    public class CreateResourceController : Controller
    {
        private static string OpenFHIRUrl = ConfigurationManager.AppSettings["OpenFHIRUrl"];
        private static FhirJsonSerializer fhirJsonSerializer = new FhirJsonSerializer();

        // GET: CreateResource
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GenerateResource(Models.CreateResourceViewModel model)
        {

            var prefix = model.patientPrefix;
            var firstName = model.patientFirstname;
            var familyName = model.patientFamilyName;
            var gender = model.patientGender;
            var dob = model.patientDateOfBirth;

            model.myPatient = new Patient();

            //Patient's Name
            model.patientName = new HumanName();
            model.patientName.Use = HumanName.NameUse.Official;
            model.patientName.Prefix = new string[] { prefix };
            model.patientName.Given = new string[] { firstName };
            model.patientName.Family = familyName;
            model.myPatient.Gender = gender == "Male" ? AdministrativeGender.Male : AdministrativeGender.Female;
            model.myPatient.BirthDate = dob;
            model.myPatient.Name = new List<HumanName>();
            model.myPatient.Name.Add(model.patientName);

            //Patient Identifier 
            model.patientIdentifier = new Identifier();
            model.patientIdentifier.System = "http://ns.electronichealth.net.au/id/hi/ihi/1.0";
            model.patientIdentifier.Value = "8003608166690503";
            model.myPatient.Identifier = new List<Hl7.Fhir.Model.Identifier>();
            model.myPatient.Identifier.Add(model.patientIdentifier);

            //Extensions
            var raceExtension = new Extension();
            raceExtension.Url = "http://hl7api.sourceforge.net/hapi-fhir/res/raceExt.html";
            raceExtension.Value = new Code { Value = "WHITE" };

            //raceExtension.AddExtension("http://hl7api.sourceforge.net/hapi-fhir/res/raceExt.html", new FhirDecimal { Value = 1.2M }, true);

            model.myPatient.Extension.Add(raceExtension);

            //Create a client to send to the server at a given endpoint.
            var FhirClient = new Hl7.Fhir.Rest.FhirClient(OpenFHIRUrl);

            try
            {
                //Validating the Patient Resource Model we created against the structure definition on the server
                model.validatePatientResource = FhirClient.ValidateCreate(model.myPatient);

                model.ReturnedPatient = FhirClient.Create<Patient>(model.myPatient);

                FhirJsonSerializer fhirJsonSerializer = new FhirJsonSerializer();

                //Data that we are sending to the server
                string jsonData = fhirJsonSerializer.SerializeToString(model.myPatient);
                model.PatientJsonDataSent = JValue.Parse(jsonData).ToString();

                //Data that we getting from the server
                jsonData = fhirJsonSerializer.SerializeToString(model.ReturnedPatient);
                model.PatientJsonDataRecieved = JValue.Parse(jsonData).ToString();
            }

            catch (FhirOperationException FhirOpExec)
            {
                var response = FhirOpExec.Outcome;
                var errorDetails = fhirJsonSerializer.SerializeToString(response);
                model.ResourceRawJsonData = JValue.Parse(errorDetails).ToString();
            }
            catch (WebException ex)
            {
                var response = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                var error = JsonConvert.DeserializeObject(response);
                model.ResourceRawJsonData = error.ToString();
            }

            return View(model);
        }
    }
}