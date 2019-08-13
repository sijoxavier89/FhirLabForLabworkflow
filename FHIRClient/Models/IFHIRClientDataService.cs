using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHIRClient.Models
{
    public interface IFHIRClientDataService
    {
        FHIRViewModel GetResourceDetails(string patientId, string resouceInfo);
    }
}