using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AsiaLabv1.Models
{
    public class ReciptModel
    {

        public PatientModel PatientDetails { get; set; }
        public string LogedInUser { get; set; }
        public IList<TestSubCategoryModel> PatientTests{ get; set; }
        public BranchModel Branch { get; set; }
      
    }
}