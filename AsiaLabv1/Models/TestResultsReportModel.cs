using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AsiaLabv1.Models
{
    public class TestResultsReportModel
    {
        public List<PatientReportModel> tests { get; set; }
        public ReferredModel ReferDoc { get; set; }
        public PatientModel PatientInfo { get; set; }
        public string Branch { get; set; }
        public UserModel PatientDoctor { get; set; }
    }
}