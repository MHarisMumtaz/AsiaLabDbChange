using AsiaLabv1.Models;
using AsiaLabv1.Services;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AsiaLabv1.Controllers
{
    public class ReciptionistController : Controller
    {
        UserService UserServices = new UserService();
        PatientService PatientServices = new PatientService();
        PatientTestService PatientTestService = new PatientTestService();
        GenderService GenderServices = new GenderService();
        TestDeptService TestDeptServices = new TestDeptService();
        TestCategoryService TestCategoryServices = new TestCategoryService();
        TestSubCategoryService TestSubCategoryServices = new TestSubCategoryService();
        ReferDoctorsService ReferDoctorsServices = new ReferDoctorsService();
        PatientPaymentService PatientPaymentServices = new PatientPaymentService();
        BranchService BranchServices = new BranchService();
        DoctorPatientsTestService DoctorPatientServices = new DoctorPatientsTestService();
        SMSService SmsServices = new SMSService();
        PaymentService PaymentServices = new PaymentService();

        public ActionResult PrintReport()
        {
            return View();
        }

        public ActionResult RegisterPatient()
        {
            if (Session["loginusername"] == null)
            {
                return RedirectToAction("LoginPage", "Main");
            }
            //:hello
            var model = new PatientModel();
            var Genders = GenderServices.GetAll();
            foreach (var item in Genders)
            {
                model.Genders.Add(new SelectListItem
                {
                    Text = item.GenderDescription,
                    Value = item.Id.ToString()
                });
            }
            var allDept = TestDeptServices.GetAllDept();

            foreach (var Dept in allDept)
            {
                model.Departments.Add(new SelectListItem
                {
                    Value = Dept.Id.ToString(),
                    Text = Dept.DepartmentName
                });
            }

            var allrefers = ReferDoctorsServices.GetAllReferDoctors();
            foreach (var refer in allrefers)
            {
                model.ReferredDoctors.Add(new SelectListItem
                {
                    Value = refer.Id.ToString(),
                    Text = refer.ReferredDoctorName
                });
            }

            var Paytypes = PatientPaymentServices.GetAllPayTypes();
            foreach (var Paytype in Paytypes)
            {
                model.PayTypes.Add(new SelectListItem
                {
                    Value = Paytype.Id.ToString(),
                    Text = Paytype.Description
                });
            }

            return View("RegisterPatient", model);
        }

        [HttpPost]
        public ActionResult GetTests(int Id)
        {
            var Tests = TestSubCategoryServices.GetSubCategTestsByTestCategoryId(Id);
            var TestList = new List<TestSubCategoryModel>();

            #region for test code delete later
            //for (int i = 0; i < 10; i++)
            //{
            //    TestList.Add(new TestSubCategoryModel
            //    {
            //        Id = i,
            //        Rate = i*5,
            //        TestSubcategoryName = "Testname"+i.ToString()
            //    });
            //}
            #endregion

            foreach (var item in Tests)
            {
                TestList.Add(new TestSubCategoryModel
                {
                    Id = item.Id,
                    Rate = item.Rate,
                    TestSubcategoryName = item.TestSubcategoryName
                });
            }

            return Json(TestList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetSubCategory(int Id)
        {
            var TestList = TestCategoryServices.GetTestCategoryByDeptId(Id);
            var TestsList = new List<Tests>();
            foreach (var item in TestList)
            {
                TestsList.Add(new Tests
                {
                    Id = item.Id,
                    Name = item.TestName
                });
            }
            return Json(TestsList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddPatient(PatientModel model)
        {

            #region temp code this must be removed
            //PatientPaymentServices.AddPayType(new PayType
            //{
            //    Description = "Card"
            //});
            //PatientPaymentServices.AddPayType(new PayType
            //{
            //    Description = "Cash"
            //});
            #endregion
            #region testing code should be delete
            //var model=new PatientModel(){
            //    BranchId=1,
            //    Name="firstTestPatient",
            //    Email="Testpatient@gmail.com",
            //    GenderId=1,
            //    DateofBirth=DateTime.Now,
            //    PhoneNumber="987987697",
            //    ReferredId=-1
            //};
            #endregion

            try
            {
                if (model.PatientTestIds.Count > 0)
                {
                    int UserId = Convert.ToInt32(Session["loginuser"].ToString());
                    var branch = UserServices.GetUserBranch(UserId);
                    model.BranchId = branch.Id;
                    model.Age = (DateTime.Now.Year - model.DateofBirth.Year).ToString();

                    PatientServices.Add(model);
                    List<TestSubcategory> selectedTests = new List<TestSubcategory>();
                    double netAmount = 0;
                    foreach (var TestId in model.PatientTestIds)
                    {
                        PatientTestService.Add(new PatientTest
                        {
                            PatientId = model.Id,
                            TestSubcategoryId = TestId
                        });
                        var test = TestSubCategoryServices.getById(TestId);
                        selectedTests.Add(test);
                        netAmount = netAmount + test.Rate;
                    }

                    if (model.Discount > 0)
                    {
                        netAmount = netAmount - model.Discount;
                    }

                    PatientPaymentServices.Add(new Payment
                    {
                        PatientId = model.Id,
                        PatientName = model.Name,
                        PaidAmount = model.PaidAmount,
                        Discount = model.Discount,
                        NetAmount = netAmount,
                        Balance = netAmount - model.PaidAmount,
                        PayTypeId = model.PayId
                    });

                    var gender = GenderServices.GetById(model.GenderId);

                    model.Genders.Add(new SelectListItem
                    {
                        Text = gender.GenderDescription
                    });

                    var referdoctor = ReferDoctorsServices.GetReferDoctorById(model.ReferredId);

                    model.ReferredDoctors.Add(new SelectListItem
                    {
                        Text = referdoctor.ReferredDoctorName
                    });
                    var branchContact = BranchServices.GetBranchContact(branch.Id);
                    
                    SmsServices.SendSms(model.PhoneNumber, model.Id);

                    var pdfDocModel = GeneratePatientRecipt(model, gender, selectedTests, branch, branchContact);

                    return Json(pdfDocModel, JsonRequestBehavior.AllowGet);
                }
                return Json("Please assign tests to patients", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult GetPatientList(PatientSearchModel model)
        {
            List<PatientModel> Patientlist = null;
            Patientlist = PatientServices.SearchPatient(Convert.ToInt32(Session["BranchId"]));

            if (model.date != null)
            {
                Patientlist = Patientlist.Where(p => p.Date.Value.ToShortDateString() == model.date.Value.ToShortDateString()).ToList();
            }
            if (model.Name != null)
            {
                Patientlist = Patientlist.Where(p => p.Name.Contains(model.Name)).ToList();
            }
            if (model.PatientId > 0)
            {
                Patientlist = Patientlist.Where(P => P.Id == model.PatientId).ToList();
            }

            return Json(Patientlist, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Recipt(int Id)
        {
            try
            {
                var patient = PatientServices.GetByPatientId(Id);
                var gender = GenderServices.GetById(patient.GenderId);
                var branch = BranchServices.GetById(patient.BranchId);
                var branchContact = BranchServices.GetBranchContact(patient.BranchId);
                var refer = ReferDoctorsServices.GetPatientReferById(patient.Id);
                var SubTestList = PatientTestService.GetSubCategoryByPatientId(patient.Id);
                var payment = PaymentServices.GetPaymentByPatientId(patient.Id);

                var model = new PatientModel
                {
                    Id = patient.Id,
                    Name = patient.PatientName,
                    Date = patient.DateTime,
                    Age = patient.Age,
                    Email = patient.Email,
                    Discount = payment.Discount,
                    PaidAmount = payment.PaidAmount,
                    PhoneNumber = patient.PatientNumber
                };

                var pdfDocModel = GeneratePatientRecipt(model, gender, SubTestList, branch, branchContact);

                return Json(pdfDocModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
            
        }

        [HttpGet]
        public ActionResult Report(int PatientId)
        {
            try
            {
                string ReturnMessage = "Report Generated";
                var TestReportDetailsList = PatientTestService.GetPatientTestsDetails(PatientId);

                var ReferDoctor = ReferDoctorsServices.GetPatientReferById(PatientId);
                var BranchName = Session["branch"].ToString();
                var Patient = PatientServices.GetByPatientId(PatientId);
                var PatientDoctor = DoctorPatientServices.GetDoctorByPatientId(PatientId);

                GeneratePatientReport(TestReportDetailsList, ReferDoctor, Patient, PatientDoctor, BranchName);

                return Json(ReturnMessage, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [NonAction]
        public void GeneratePatientReport(List<PatientReportModel> model, Refer ReferDoctor, Patient PatientInfo, UserEmployee PatientDoctor, string BranchName)
        {
            //StreamWriter sq = new StreamWriter(Server.MapPath("/images/") + "log.txt");
            //sq.Close();
            //StreamWriter sw = new StreamWriter(Server.MapPath("/images/") + "log.txt");
            //sw.WriteLine("patient report method start....");
            string filename = "Report-" + PatientInfo.Id + ".pdf";
            if (!System.IO.File.Exists(Request.MapPath("/PatientsReport/") + filename))
            {
                
                var path = Server.MapPath("/images/");
               // sw.WriteLine("condition true of patient report. file doesn't exist..");
               // sw.WriteLine("class going to instantiate report.");
                PatientReport Report = new PatientReport(path, model, ReferDoctor, PatientInfo, PatientDoctor, BranchName);
                //sw.WriteLine("report class instantiated");
                PdfDocument pdf = Report.CreateDocument();
                //sw.WriteLine("report is created.");

                pdf.Save(Server.MapPath("/PatientsReport/") + filename);
                //sw.WriteLine("report saved successfully");
                //    System.IO.FileInfo fi=new System.IO.FileInfo(Request.MapPath("/PatientsReport/") + filename);
                //    fi.Delete();
            }
            // ...and start a viewer.
            //sw.WriteLine("report process start.");
            Process.Start(Server.MapPath("/PatientsReport/") + filename);
            //sw.WriteLine("report PROCESS END");
            //sw.Close();
        }

        [NonAction]
        public PdfDocModel GeneratePatientRecipt(PatientModel model, Gender gender, List<TestSubcategory> tests, Branch branch, Contact branchcontact)
        {
            //StreamWriter sq = new StreamWriter(Server.MapPath("/images/") + "logreceipt.txt");
            //sq.Close();
            //StreamWriter sw = new StreamWriter(Server.MapPath("/images/") + "logreceipt.txt");
            //sw.WriteLine("patient receipt method start....");           
            try
            {
                string filename = "Recipt-" + model.Id + ".pdf";
                
                var pdfDocModel = new PdfDocModel() { ErrorObject = null, PdfDoc = null };

                if (!System.IO.File.Exists(Server.MapPath("/PatientsReport/") + filename))
                {
                    var path = Server.MapPath("/images/");
                    //sw.WriteLine("condition true of patient receipt. file doesn't exist..");
                    //sw.WriteLine("class going to instantiate receipt.");
                    PatientRecipt recipt = new PatientRecipt(path, Session["loginusername"].ToString(), gender, model, tests, branch, branchcontact);
                    //   sw.WriteLine("receipt class instantiated");
                    pdfDocModel = recipt.CreateDocument();
                    if (pdfDocModel.PdfDoc != null)
                    {
                        pdfDocModel.PdfDoc.Save(Server.MapPath("/PatientsReport/") + filename);
                        Process.Start(Server.MapPath("/PatientsReport/") + filename);
                    }
                    //    sw.WriteLine("receipt is created.");


                    ////////////pdf.Save(Server.MapPath("/PatientsReport/") + filename);


                    //  sw.WriteLine("receipt saved successfully");
                    //    System.IO.FileInfo fi=new System.IO.FileInfo(Request.MapPath("/PatientsReport/") + filename);
                    //    fi.Delete();
                }
                else
                {
                    Process.Start(Server.MapPath("/PatientsReport/") + filename);
                }
                return pdfDocModel;
            }
            catch (Exception ex)
            {
                return new PdfDocModel()
                {
                    PdfDoc = null,
                    ErrorObject = ex
                };
            }
            //sw.WriteLine("receipt process start.");
            ////////////////Process.Start(Server.MapPath("/PatientsReport/") + filename);
            //sw.WriteLine("receipt PROCESS END");
            //sw.Close();

          

            //var path = Server.MapPath("/images/");

            //PatientRecipt recipt = new PatientRecipt(path, Session["loginusername"].ToString(), gender, model, tests, branch, branchcontact);

            //recipt.Check();

            //string filename = "Recipt-" + model.Id + ".pdf";
            //if (!System.IO.File.Exists(Request.MapPath("/PatientsReport/") + filename))
            //{
            //    PdfDocument pdf = recipt.CreateDocument();

            //    pdf.Save(Server.MapPath("/PatientsReport/") + filename);
            //    //    System.IO.FileInfo fi=new System.IO.FileInfo(Request.MapPath("/PatientsReport/") + filename);
            //    //    fi.Delete();
            //}
            //Process.Start(Server.MapPath("/PatientsReport/") + filename);
        }
    }
}
