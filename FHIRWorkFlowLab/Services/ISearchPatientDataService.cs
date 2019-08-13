using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHIRWorkFlowLab.Models.SearchPatient
{
    public interface ISearchPatientDataService 
    {
        SearchPatientViewModel GetPatientDetails(string patientId);
    }
}