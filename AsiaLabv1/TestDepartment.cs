//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AsiaLabv1
{
    using System;
    using System.Collections.Generic;
    
    public partial class TestDepartment
    {
        public TestDepartment()
        {
            this.TestCategories = new HashSet<TestCategory>();
        }
    
        public int Id { get; set; }
        public string DepartmentName { get; set; }
    
        public virtual ICollection<TestCategory> TestCategories { get; set; }
    }
}
