using FHIRClient.Models.ResourceHistory;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace FHIRClient.Controllers
{
    public class ResourceHistoryController : Controller
    {
        private static string OpenFHIRUrl = ConfigurationManager.AppSettings["OpenFHIRUrl"];

        private static FhirClient FhirClient = new FhirClient(OpenFHIRUrl);

        private static FhirJsonParser fhirJsonParser = new FhirJsonParser();
        private static FhirJsonSerializer fhirJsonSerializer = new FhirJsonSerializer();


        // GET: ResourceHistory
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(ResourceHistoryViewModel model)
        {
            var patientId = model.patientID;

            try
            {
                // We have hard-coded the resource type to Patient to get history details of the patient 
                var historyRead = FhirClient.History(ResourceIdentity.Build("Patient", patientId));

                string _history = fhirJsonSerializer.SerializeToString(historyRead);

                Bundle historyBundle = (Bundle)fhirJsonParser.Parse(_history, typeof(Bundle));

                model.OperationOutcomePatient = historyBundle.Entry.Select(Resource => (Patient)Resource.Resource).ToList();

                //Displaying the Div element of the text section
                foreach (var hist in model.OperationOutcomePatient)
                {
                    ViewBag.operationOutcomeText = hist.Text == null ? string.Empty : hist.Text.Div;
                }

                ViewBag.patientHistory = JValue.Parse(_history).ToString();
            }
            catch (FhirOperationException FhirOpExec)
            {
                var response = FhirOpExec.Outcome;
                var errorDetails = fhirJsonSerializer.SerializeToString(response);
                ViewBag.error = JValue.Parse(errorDetails).ToString();
            }

            return View();
        }
    }
}