using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Mvc;

namespace FHIRClient.Controllers
{
    public class UpdateResourceController : Controller
    {
        private static string OpenFHIRUrl = ConfigurationManager.AppSettings["OpenFHIRUrl"];
        private static FhirJsonSerializer fhirJsonSerializer = new FhirJsonSerializer();
        private static FhirJsonParser fhirJsonParser = new FhirJsonParser();

        // GET: UpdateResource
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateResource(Models.UpdateResourceViewModel model)
        {
            try
            {
                var patientID = model.patientID;

                //Create a client to send to the server at a given endpoint.
                var FhirClient = new Hl7.Fhir.Rest.FhirClient(OpenFHIRUrl);
                var ResourceUpdate = ("/Patient/" + patientID);

                //Reading the data at the given location("URL") 
                model.patientRead = FhirClient.Read<Patient>(ResourceUpdate);

                //using Json serializer to get the data in Json format
                FhirJsonSerializer fhirJsonSerializer = new FhirJsonSerializer();

                //Serializing the existing patient 
                string oldDetails = fhirJsonSerializer.SerializeToString(model.patientRead);
                model.patientDetailsOld = JValue.Parse(oldDetails).ToString();

                //Updating the changes to the existing Resource

                model.patientFullnameUpdate = new HumanName();
                model.patientFullnameUpdate.Use = HumanName.NameUse.Official;
                model.patientFullnameUpdate.Prefix = new string[] { model.patientPrefixUpdate };
                model.patientFullnameUpdate.Given = new string[] { model.patientFirstnameUpdate };
                model.patientFullnameUpdate.Family = model.patientFamilyNameUpdate;
                model.patientRead.Name = new List<HumanName>();
                model.patientRead.Name.Add(model.patientFullnameUpdate);
                model.patientRead.Gender = model.patientGenderUpdate == "Male" ? AdministrativeGender.Male : AdministrativeGender.Female;
                model.patientRead.BirthDate = model.patientDateOfBirthUpdate;

                //Sending the updated changes to the endpoint
                model.patientupdate = FhirClient.Update<Patient>(model.patientRead);

                // Serializing the updated patient details
                string Updatedetails = fhirJsonSerializer.SerializeToString(model.patientupdate);
                model.patientDetailsUpdated = JValue.Parse(Updatedetails).ToString();
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