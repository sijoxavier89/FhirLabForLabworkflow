using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHIRClient.Models
{
    public interface ISearchResourceDataService
    {
        SearchResourceViewModel GetResourceDetails(string patientId, string resouceInfo);
    }
}