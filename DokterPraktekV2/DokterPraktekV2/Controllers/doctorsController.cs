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

namespace DokterPraktekV2.Controllers
{
    public class doctorsController : Controller
    {
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();
        private DoctorService doctorService = new DoctorService(); 
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
