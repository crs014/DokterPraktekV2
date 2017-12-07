using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DokterPraktekV2;
using System.Web.Security;
using Microsoft.AspNet.Identity;

namespace DokterPraktekV2.Controllers
{
    public class medicinesController : Controller
    { 
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();
        
        // GET: medicines
        public ActionResult Index()
        {
            var idLog = User.Identity.GetUserId();
            var ids = db.doctors.Where(m => m.userId == idLog);
            foreach (var item in ids)
            {
                    ViewBag.id = item.id;
            }
            var medicine = db.medicines.Include(m => m.doctor);
            return View(medicine.ToList());
        }

        // GET: medicines/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            medicine medicine = db.medicines.Find(id);
            if (medicine == null)
            {
                return HttpNotFound();
            }
            return View(medicine);
        }

        // GET: medicines/Create
        public ActionResult Create(int id)
        {
            var namaid = db.doctors.Find(id).name;
            var idx = db.doctors.Find(id).id;
            ViewBag.idx = idx;
            ViewBag.ID = namaid;
            return View();
        }

        // POST: medicines/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "doctorId,name,price,quantity,dateIn,expired")] medicine medicine,[Bind(Include ="id,medicineId,doctorId,statusTransaction,quantity")] medicineTransaction medtrans)
        {
            medicine.dateIn = DateTime.Today;
            ViewBag.getData = db.medicineTransactions;
            //if(medtrans.statusTransaction == true)
            //{
            //    medtrans.statusTransaction = true;
            //}else
            //{
            //    medtrans.statusTransaction = true;
            //}
            if (ModelState.IsValid)
            {
                medtrans.statusTransaction = true;
                db.medicineTransactions.Add(medtrans);
                db.medicines.Add(medicine);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.doctorId = new SelectList(db.doctors, "id", "name", medicine.doctorId);
            return View(medicine);
        }

        // GET: medicines/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            medicine medicine = db.medicines.Find(id);
            if (medicine == null)
            {
                return HttpNotFound();
            }
            ViewBag.doctorId = new SelectList(db.doctors, "id", "name", medicine.doctorId);
            return View(medicine);
        }

        // POST: medicines/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,doctorId,name,harga,quantity,dateIn,expired")] medicine medicine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(medicine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.doctorId = new SelectList(db.doctors, "id", "name", medicine.doctorId);
            return View(medicine);
        }

        // GET: medicines/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            medicine medicine = db.medicines.Find(id);
            if (medicine == null)
            {
                return HttpNotFound();
            }
            return View(medicine);
        }

        // POST: medicines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            medicine medicine = db.medicines.Find(id);
            db.medicines.Remove(medicine);
            db.SaveChanges();
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
