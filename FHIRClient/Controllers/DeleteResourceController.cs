using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Mvc;

namespace FHIRClient.Controllers
{
    public class DeleteResourceController : Controller
    {
        private static string OpenFHIRUrl = ConfigurationManager.AppSettings["OpenFHIRUrl"];
        private static FhirJsonSerializer fhirJsonSerializer = new FhirJsonSerializer();

        // GET: DeleteResource
        [HttpGet]
        public ActionResult Index()
        {         
            return View();
        }

        [HttpPost]
        public ActionResult Index(Models.DeleteResourceViewModel model)
        {
            var patientID = model.patientID;
            bool success = false;

            try
            {
                //Create a client to send to the server at a given endpoint.
                var FhirClient = new Hl7.Fhir.Rest.FhirClient(OpenFHIRUrl);

                var ResourceDelete = ("/Patient/" + patientID);

                FhirClient.Delete(ResourceDelete);
                success = true;

                ViewBag.showData = success == true ? "Resource was deleted" : "Error while deleting the resource";
            }
            catch (FhirOperationException FhirOpExec)
            {
                var response = FhirOpExec.Outcome;
                var errorDetails = fhirJsonSerializer.SerializeToString(response);
                ViewBag.showData = JValue.Parse(errorDetails).ToString();
            }
            catch (WebException ex)
            {
                var response = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                var error = JsonConvert.DeserializeObject(response);
                ViewBag.showData = error.ToString();
            }

            return View();
        }
    }
}