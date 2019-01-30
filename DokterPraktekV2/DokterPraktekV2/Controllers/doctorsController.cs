using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DokterPraktekV2;
using DokterPraktekV2.Services;
using PagedList;
using DokterPraktekV2.Models;
using Microsoft.AspNet.Identity;

namespace DokterPraktekV2.Controllers
{
    public class doctorsController : Controller
    {
        private dayInServices dys = new dayInServices();
        private DokterPraktekEntities db = new DokterPraktekEntities();
        private DoctorService doctorService = new DoctorService();
        private PatientServices patientService = new PatientServices();
        private PhotoService photoService = new PhotoService();


        [Authorize(Roles = "doctor")]
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {

            var authId = User.Identity.GetUserId();
            doctor dataDoctor = db.doctors.FirstOrDefault(e => e.userId == authId);


            ViewBag.Doctor = doctorService.DoctorAuth(authId);
            /*call all data from service*/
            var data = doctorService.TodaySchedule(dataDoctor.userId);
            /*search data from name patient*/
            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(data.ToPagedList(pageNumber, pageSize));
        }
        
        [Authorize(Roles = "doctor")]
        public ActionResult InputHistory(string patientId ,string currentFilter, int? page)
        {
            //get patient data and picture patient
            var id = User.Identity.GetUserId();
            var data = doctorService.scheduleDetail(id,Convert.ToInt32(patientId));
            string mime;
            string convertedImage = photoService.LoadImage(data.PatientID, out mime);
            ViewBag.tipeImage = mime;
            ViewBag.stringUrl = convertedImage;
            ViewBag.dataPatient = data;
            ViewBag.DoctorName = db.doctors.Where(e => e.userId == data.DoctorID).FirstOrDefault().name;

            //list data patient history by patient id
            var userID = User.Identity.GetUserId();
            doctor dataDoctor = db.doctors.FirstOrDefault(e => e.userId == userID);
            var dataList = patientService.allPatientHistory(data.PatientID, dataDoctor.userId).OrderByDescending(e => e.date);
            int pageNumber = (page ?? 1);
            int pageSize = 7;

            return View(dataList.ToPagedList(pageNumber, pageSize));
        }

      

        public ActionResult GetMedicine()
        {
            var authId = User.Identity.GetUserId();
            doctor dataDoctor = db.doctors.FirstOrDefault(e => e.userId == authId);

            //request dari clientside datatables
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            //ambil data awal
            List<Medicine> medicines = new List<Medicine>();
            medicines = doctorService.getDoctorMedicine(dataDoctor.id);
            int total = medicines.Count();

            //jika searchbox tidak kosong ekseskusi dibawah
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                medicines = medicines.Where(x =>
                    x.Name.ToLower().Contains(searchValue)
                ).ToList<Medicine>();
            }
            int totalFilter = medicines.Count();

            //sorting
            //customers = customers.OrderBy(e => sortColumnName).ToList<Customer>();

            //paging 
            medicines = medicines.Skip(start).Take(length).ToList<Medicine>();

            return Json(new
            {
                recordsTotal = total,
                recordsFiltered = totalFilter,
                data = medicines,
                draw = Request["draw"]
            }, JsonRequestBehavior.AllowGet);

        }


        [ActionName("InputHistory"), HttpPost, Authorize(Roles = "doctor")]
        public ActionResult PostInputHistory(string doctorId,int patientId, VM_inputHistory history)
        {
            int id_history;
            var data = doctorService.scheduleDetail(doctorId,patientId);


            var historys = new MedicalHistory();
            historys.Sickness = history.sick;
            historys.DoctorID = data.DoctorID;
            historys.PatientID = data.PatientID;
            historys.DescriptionInfo = history.descriptionSick;
            historys.CheckUpPrice = history.price;
            historys.CheckUpDate = DateTime.Now.Date;
            db.MedicalHistories.Add(historys);
            var getId = db.Schedules.Where(e => e.DoctorID == doctorId && e.BookingStatus == "Booking" && e.PatientID == data.PatientID).FirstOrDefault().ID;
            db.Schedules.Find(getId).BookingStatus = "Completed";
            db.SaveChanges();
            id_history = historys.ID;

            try
            {
                int leng = history.medicineId.Count();
                for (var i = 0; i < leng; i++)
                {
                    var patientMedic = new PatientMedicine();
                    decimal price = history.medicineId[i];

                    patientMedic.MedicalHistoryID = id_history;
                    patientMedic.MedicineID = history.medicineId[i];
                    patientMedic.Quantity = history.quantity[i];
                    patientMedic.Description = history.describeMedic[i];
                    patientMedic.MedicalPrice = db.Medicines.Where(e => e.ID == price).First().Price;
                    db.PatientMedicines.Add(patientMedic);
                    db.SaveChanges();
                }

                for (var j = 0; j < leng; j++)
                {
                    var medicTrans = new MedicineTransaction();
                    medicTrans.MedicineID = history.medicineId[j];
                    medicTrans.DoctorID = data.DoctorID;
                    medicTrans.TransactionStatus = false;
                    medicTrans.Quantity = history.quantity[j];
                    db.MedicineTransactions.Add(medicTrans);
                    db.SaveChanges();
                }
            }
            catch(Exception e)
            {
                return RedirectToAction("Index");
            }   
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "doctor")]
        public ActionResult JsonMedicinePatient(int id)
        {
            var authId = User.Identity.GetUserId();
            var medicineList = db.PatientMedicines.Where(e => e.MedicalHistoryID == id).Select(a => new VM_patientMedicine {
                id = a.MedicineID,
                medicineName = a.Medicine.Name,
                description = a.Description,
                historyId = a.ID,
                price = a.MedicalPrice,
                quantity = a.Quantity
            }).ToList<VM_patientMedicine>();
            return Json(new { medicine = medicineList},JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
