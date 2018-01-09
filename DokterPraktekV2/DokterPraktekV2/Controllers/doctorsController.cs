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
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();
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
            var data = doctorService.TodaySchedule(dataDoctor.id);
            /*search data from name patient*/
            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(data.ToPagedList(pageNumber, pageSize));
        }
        
        [Authorize(Roles = "doctor")]
        public ActionResult InputHistory(int id, string currentFilter, int? page)
        {
            //get patient data and picture patient
            var data = doctorService.scheduleDetail(id);
            string mime;
            string convertedImage = photoService.LoadImage(data.patientId, out mime);
            ViewBag.tipeImage = mime;
            ViewBag.stringUrl = convertedImage;
            ViewBag.dataPatient = data;

            //list data patient history by patient id
            var userID = User.Identity.GetUserId();
            doctor dataDoctor = db.doctors.FirstOrDefault(e => e.userId == userID);
            var dataList = patientService.allPatientHistory(data.patientId, dataDoctor.id).OrderByDescending(e => e.date);
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
            List<medicine> medicines = new List<medicine>();
            medicines = doctorService.getDoctorMedicine(dataDoctor.id);
            int total = medicines.Count();

            //jika searchbox tidak kosong ekseskusi dibawah
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                medicines = medicines.Where(x =>
                    x.name.ToLower().Contains(searchValue)
                ).ToList<medicine>();
            }
            int totalFilter = medicines.Count();

            //sorting
            //customers = customers.OrderBy(e => sortColumnName).ToList<Customer>();

            //paging 
            medicines = medicines.Skip(start).Take(length).ToList<medicine>();

            return Json(new
            {
                recordsTotal = total,
                recordsFiltered = totalFilter,
                data = medicines,
                draw = Request["draw"]
            }, JsonRequestBehavior.AllowGet);

        }


        [ActionName("InputHistory"), HttpPost, Authorize(Roles = "doctor")]
        public ActionResult PostInputHistory(int id, VM_inputHistory history)
        {
            int id_history;
            var data = doctorService.scheduleDetail(id);


            var historys = new history();
            historys.sickness = history.sick;
            historys.doctorId = data.doctorId;
            historys.patientId = data.patientId;
            historys.descriptionInfo = history.descriptionSick;
            historys.checkupPrice = history.price;
            historys.checkupDate = DateTime.Now.Date;
            db.histories.Add(historys);
            db.schedules.Find(id).bookingStatus = "Completed";
            db.SaveChanges();
            id_history = historys.id;

            try
            {
                int leng = history.medicineId.Count();
                for (var i = 0; i < leng; i++)
                {
                    var patientMedic = new patientMedicine();
                    decimal price = history.medicineId[i];

                    patientMedic.historyId = id_history;
                    patientMedic.medicineId = history.medicineId[i];
                    patientMedic.quantity = history.quantity[i];
                    patientMedic.describe = history.describeMedic[i];
                    patientMedic.medicinePrice = db.medicines.Where(e => e.id == price).First().price;
                    db.patientMedicines.Add(patientMedic);
                    db.SaveChanges();
                }

                for (var j = 0; j < leng; j++)
                {
                    var medicTrans = new medicineTransaction();
                    medicTrans.medicineId = history.medicineId[j];
                    medicTrans.doctorId = data.doctorId;
                    medicTrans.statusTransaction = false;
                    medicTrans.quantity = history.quantity[j];
                    db.medicineTransactions.Add(medicTrans);
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
            var medicineList = db.patientMedicines.Where(e => e.historyId == id).Select(a => new VM_patientMedicine {
                id = a.medicineId,
                medicineName = a.medicine.name,
                description = a.describe,
                historyId = a.id,
                price = a.medicinePrice,
                quantity = a.quantity
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
