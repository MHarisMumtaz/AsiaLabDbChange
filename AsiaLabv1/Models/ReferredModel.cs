using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AsiaLabv1.Models
{
    public class ReferredModel
    {
        public int Id { get; set; }

        [Display(Name="Referral Name")]
        public string ReferredDoctorName { get; set; }

        [Display(Name = "Address")]
        public string ReferredDocAddress { get; set; }

        [Display(Name = "Number")]
        public string ReferredDocNumber { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Commission")]
        public Nullable<double> commission { get; set; }

         [Display(Name = "Select Department")]
        public int DeptId { get; set; }
        public List<SelectListItem> Departments { get; set; }

        public List<DoctorDeptCommision> DocDeptsCommision { get; set; }

        public ReferredModel()
        {
            this.Departments = new List<SelectListItem>();
        }

    }
    public class DoctorDeptCommision
    {
        public int DeptId { get; set; }
        public float Commision { get; set; }
    }

}