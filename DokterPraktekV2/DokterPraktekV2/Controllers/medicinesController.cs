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
using DokterPraktekV2.Models;
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
            var ids = db.doctors.Where(m => m.userId == idLog).First();
            var op = db.medicines.Where(a => a.doctorId == ids.id).ToList();
            var medicine = db.medicines.Include(m => m.doctor);

            var data = db.medicines.Select(e => new VM_Stock
            {
                id = e.id,
                doctorId = e.doctorId,
                nameMedicine = e.name,
                price = e.price,
                dateIn = e.dateIn,
                expired = e.expired,
                inStock = e.quantity,
                outStock = e.patientMedicines.Sum(a => a.quantity),
                remainStock = e.quantity - e.patientMedicines.Sum(a => a.quantity)
            }).ToList().Where(a=>a.doctorId == ids.id);
            ViewBag.Data = data;

            DateTime estimatedDate;
            estimatedDate = DateTime.Now.Date.AddDays(30);
            List<string> listWarna = new List<string>();
            List<string> listStatus = new List<string>();
            foreach (var item in data)
            {
                if (estimatedDate >= item.expired)
                {
                    var warna = "red";
                    var status = "Expired in 1 Month";
                    listWarna.Add(warna);
                    listStatus.Add(status);
                }
                else
                {
                    var warna = "green";
                    var status = "Secure";
                    listWarna.Add(warna);
                    listStatus.Add(status);
                    
                }
            }
            ViewBag.warna = listWarna;
            ViewBag.status = listStatus;
            return View(op);
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: medicines/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name,price,quantity,dateIn,expired")] medicine medicine,[Bind(Include ="id,medicineId,statusTransaction,quantity")] medicineTransaction medtrans)
        {
            var idLog = User.Identity.GetUserId();
            var ids = db.doctors.Where(m => m.userId == idLog).First();
            medicine.dateIn = DateTime.Today;
            ViewBag.getData = db.medicineTransactions;
            if (ModelState.IsValid)
            {
                medtrans.doctorId = ids.id;
                medtrans.statusTransaction = true;
                db.medicineTransactions.Add(medtrans);
                medicine.doctorId = ids.id;
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
        public ActionResult Stock()
        {
           
            var idLog = User.Identity.GetUserId();
            var ids = db.doctors.Where(m => m.userId == idLog).First();
            var op = db.medicines.Where(a => a.doctorId == ids.id).ToList();

            var data = db.medicines.Select(e => new VM_Stock
            {
                id = e.id,
                doctorId = e.doctorId,
                nameMedicine = e.name,
                price = e.price,
                dateIn = e.dateIn,
                expired = e.expired,
                inStock = e.quantity,
                outStock = e.patientMedicines.Sum(a => a.quantity),
                remainStock = e.quantity - e.patientMedicines.Sum(a=>a.quantity)
            }).ToList();
            ViewBag.Data = data;
            
            return View(data);
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
