﻿using AsiaLabv1.Models;
using AsiaLabv1.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AsiaLabv1.Services
{
    public class PatientTestService
    {

        Repository<TestDepartment> _TestDeptRepository = new Repository<TestDepartment>();

        Repository<PatientTest> _PatientTestRepository = new Repository<PatientTest>();
        Repository<TestSubcategory> _TestSubCategoryRepository = new Repository<TestSubcategory>();
        Repository<Patient> _PatientRepository = new Repository<Patient>();
        Repository<PatientTestResult> _PatientTestResultRepository = new Repository<PatientTestResult>();
        Repository<TechnicianPatientsTest> _TechnicianPatientTestRepository = new Repository<TechnicianPatientsTest>();
        Repository<DoctorPatientsTest> _DoctorsPatientsTestsRepository = new Repository<DoctorPatientsTest>();
        Repository<DoctorComment> _DoctorCommentsRepository = new Repository<DoctorComment>();
        Repository<TestCategory> _TestCategoryRepository = new Repository<TestCategory>();
        Repository<Branch> _BranchRepository = new Repository<Branch>();

        public void Add(PatientTest Patienttest)
        {
            _PatientTestRepository.Insert(Patienttest);
        }

        public List<Patient> GetPatientTests(int branchid)
        {
            var check2 = (from pt in _PatientTestRepository.Table
                          join tr in _PatientTestResultRepository.Table
                          on pt.Id equals tr.PatientTestId
                          select pt.PatientId).ToList();

            var query = (from p in _PatientRepository.Table
                         join pt in _PatientTestRepository.Table
                         on p.Id equals pt.PatientId
                         where check2.Contains(pt.PatientId)==false && p.BranchId==branchid
                         select p).ToList<Patient>().GroupBy(test => test.Id).Select(grp => grp.First()).ToList();
            return query;
        }


        public List<Patient> GetPatientTestsUpdate(string approvalstatus)
        {
            var check2 = (from pt in _PatientTestRepository.Table
                          join tr in _PatientTestResultRepository.Table
                          on pt.Id equals tr.PatientTestId
                          where tr.ApprovalStatus == approvalstatus
                          select pt.PatientId).ToList();

            var query = (from p in _PatientRepository.Table
                         join pt in _PatientTestRepository.Table
                         on p.Id equals pt.PatientId
                         where check2.Contains(pt.PatientId)
                         select p).ToList<Patient>().GroupBy(test => test.Id).Select(grp => grp.First()).ToList();

            return query;
        }

        public int GetBranchId(string Name)
        {
            int query = (from b in _BranchRepository.Table
                         where b.BranchName == Name
                         select b.Id).FirstOrDefault();
            return query;
        }

        public List<Patient> GetPatientTestsDoctor(String deptname)
        {
            var abc = (from ptr in _PatientTestResultRepository.Table
                       where ptr.ApprovalStatus == "Approved" || ptr.ApprovalStatus == "Rejected"
                       select ptr.PatientTestId).ToList();

            var check2 = (from pt in _PatientTestRepository.Table
                          join tr in _PatientTestResultRepository.Table
                          on pt.Id equals tr.PatientTestId
                          where abc.Contains(tr.PatientTestId) == false && pt.TestSubcategory.TestCategory.TestDepartment.DepartmentName == deptname
                          select pt.PatientId).ToList();


            var query = (from p in _PatientRepository.Table
                         join pt in _PatientTestRepository.Table
                         on p.Id equals pt.PatientId
                         where check2.Contains(pt.PatientId)
                         select p).ToList<Patient>().GroupBy(test => test.Id).Select(grp => grp.First()).ToList();
            return query;
        }

        public List<PatientTest> GetPatientTestsByPatientId(int PatientId)
        {
            var query = (from pt in _PatientTestRepository.Table
                         join t in _TestSubCategoryRepository.Table
                         on pt.TestSubcategoryId equals t.Id
                         where pt.PatientId == PatientId
                         select pt).ToList<PatientTest>();
            return query;
        }


        public void InsertPatientTestResults(PatientTestResult model)
        {
            _PatientTestResultRepository.Insert(model);
        }

        public void InsertTechnicianPatientTests(TechnicianPatientsTest model)
        {
            _TechnicianPatientTestRepository.Insert(model);
        }

        public void UpdateTest(int id, string status)
        {

            var iid = (from p in _PatientTestRepository.Table
                       where p.PatientId == id
                       select p.Id).ToList();

            List<PatientTestResult> original = (from ptr in _PatientTestResultRepository.Table
                                                where iid.Contains(ptr.PatientTestId)
                                                select ptr).ToList<PatientTestResult>();

            if (original != null)
            {
                foreach (var item in original)
                {
                    item.ApprovalStatus = status;
                    _PatientTestResultRepository.UpdateGeneric(item);
                }

            }
        }

        public void UpdateRejectedTest(int id, string[] tests)
        {

            var original = (from ptr in _PatientTestResultRepository.Table
                                                where ptr.PatientTestId == id
                                                select ptr).ToList<PatientTestResult>();

            if (original != null)
            {
                int count = 0;
                foreach (var item in original)
                {
                    item.Result = tests[count];
                    item.ApprovalStatus = "Not Approved";
                    _PatientTestResultRepository.UpdateGeneric(item);
                    count++;
                }

            }
        }

        public void InsertDoctorsPatientsTests(DoctorPatientsTest model)
        {
            _DoctorsPatientsTestsRepository.Insert(model);
        }

        public void InsertDoctorComments(DoctorComment model)
        {
            _DoctorCommentsRepository.Insert(model);
        }

        public List<string> GetTestResultsById(int id)
        {
            var query = (from ptr in _PatientTestResultRepository.Table
                         where ptr.PatientTestId == id
                         select ptr.Result).ToList();
            return query;
        }

        public List<PatientReportModel> GetPatientTestsDetails(int PatientId)
        {
            var Query = (from Ptest in _PatientTestRepository.Table
                         join TestSubcat in _TestSubCategoryRepository.Table on Ptest.TestSubcategoryId equals TestSubcat.Id
                         join TestCat in _TestCategoryRepository.Table on TestSubcat.TestCategoryId equals TestCat.Id
                         join TestDept in _TestDeptRepository.Table on TestCat.TestDepartmentId equals TestDept.Id
                         join PtestResult in _PatientTestResultRepository.Table on Ptest.Id equals PtestResult.PatientTestId
                         where Ptest.PatientId == PatientId
                         select new
                         {
                             DeptId = TestDept.Id,
                             DeptName = TestDept.DepartmentName,
                             CatId = TestCat.Id,
                             TestCatName = TestCat.TestName,
                             TestSubCatId = TestSubcat.Id,
                             TestSubCatName = TestSubcat.TestSubcategoryName,
                             LowerBound = TestSubcat.LowerBound,
                             UpperBound = TestSubcat.UpperBound,
                             result = PtestResult.Result,
                             Unit = TestSubcat.Unit
                         }).ToList();
            var list = new List<PatientReportModel>();
            var SubcatList = GetSubCategoryByPatientId(PatientId);
            SubcatList = SubcatList.OrderBy(S => S.Id).ToList();
            Query = Query.OrderBy(P => P.TestSubCatId).ToList();
            var newlist = Query.Zip(SubcatList, (first, second) => new { Q = first, S = second });
            foreach (var item in newlist)
            {
                list.Add(new PatientReportModel
                {
                    DepartmentId = item.Q.DeptId,
                    CategoryId = item.Q.CatId,
                    TestSubCategoryId = item.Q.TestSubCatId,
                    DepartmentName = item.Q.DeptName,
                    TestSubCategoryName = item.S.TestSubcategoryName,
                    TestCategoryName = item.Q.TestCatName,
                    LowerBound = item.S.LowerBound,
                    UpperBound = item.S.UpperBound,
                    Result = item.Q.result,
                    Unit = item.S.Unit
                });
            }
            return list;
        }

        public string GetComment(int patientid)
        {
            var query = (from dc in _DoctorCommentsRepository.Table
                         where dc.PatientId == patientid
                         select dc).ToList();
            if (query.Count > 0) { return query.LastOrDefault().Comments; }
            return "";
        }

        public List<TestSubcategory> GetSubCategoryByPatientId(int patientId)
        {
            var Query = (from PT in _PatientTestRepository.Table
                         join Subcat in _TestSubCategoryRepository.Table on PT.TestSubcategoryId equals Subcat.Id
                         where PT.PatientId == patientId
                         select Subcat).ToList();
            return Query;
        }
    }
}