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

namespace DokterPraktekV2.Controllers
{
    public class doctorsController : Controller
    {
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();
        private DoctorService doctorService = new DoctorService();
        private PatientServices patientService = new PatientServices();
        // GET: doctors
        public ActionResult Index()
        {
            return View(db.doctors.ToList());
        }

        // GET: doctors/Details/5
        public ActionResult Details(int? id)
        {
            var getName = db.doctors.Find(id).name;
            var getGen = db.doctors.Find(id).gender;
            var getMed = db.medicines.Where(m => m.doctorId == id);
            ViewBag.getGen = getGen;
            ViewBag.med = getMed;
            ViewBag.nameMed = getName;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            doctor doctor = db.doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // GET: doctors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: doctors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,homeAddress,gender,phone,registerDatetime")] doctor doctor)
        {
            if (ModelState.IsValid)
           { 
                db.doctors.Add(doctor);
                db.SaveChanges();
                doctorService.createDays(doctor.id);
                return RedirectToAction("Index");
            }

            return View(doctor);
        }

        // GET: doctors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            doctor doctor = db.doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // POST: doctors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,homeAddress,gender,phone,registerDatetime")] doctor doctor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doctor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctor);
        }

        // GET: doctors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            doctor doctor = db.doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // POST: doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            doctor doctor = db.doctors.Find(id);
            db.doctors.Remove(doctor);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        

        
        public ActionResult TodaySchedule(int id,string currentFilter, string searchString, int? page)
        {
            ViewBag.Doctor = doctorService.doctorDetail(id);
            /*call all data from service*/
            var data = doctorService.TodaySchedule(id);
            /*search data from name patient*/
            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult InputHistory(int id)
        {  
            var data = doctorService.scheduleDetail(id);
            ViewBag.MedicineList = doctorService.getDoctorMedicine(data.doctorId);
            return View(data);
        }

        [ActionName("InputHistory"), HttpPost]
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

          
            if(history.medicineId != null)
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
            return RedirectToAction("Index");
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
