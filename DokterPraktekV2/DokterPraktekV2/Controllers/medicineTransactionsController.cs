﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DokterPraktekV2;
using Microsoft.AspNet.Identity;

namespace DokterPraktekV2.Controllers
{
    public class medicineTransactionsController : Controller
    {
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();

        // GET: medicineTransactions
        public ActionResult Index()
        {
            var idLog = User.Identity.GetUserId();
            var ids = db.doctors.Where(m => m.userId == idLog).First();
            var op = db.medicineTransactions.Where(a => a.doctorId == ids.id).ToList();

            var b = db.medicineTransactions.Include(m => m.medicineId);
            ViewBag.a = b;
            var getMed = db.medicines.Include(a => a.doctor).Include(a => a.medicineTransactions);
            var medicineTransaction = db.medicineTransactions.Include(m => m.doctor).Include(m => m.medicine);
            return View(op);
        }

        // GET: medicineTransactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            medicineTransaction medicineTransaction = db.medicineTransactions.Find(id);
            if (medicineTransaction == null)
            {
                return HttpNotFound();
            }
            return View(medicineTransaction);
        }

        // GET: medicineTransactions/Create
        public ActionResult Create()
        {
            ViewBag.doctorId = new SelectList(db.doctors, "id", "name");
            ViewBag.medicineId = new SelectList(db.medicines, "id", "name");
            return View();
        }

        // POST: medicineTransactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,medicineId,doctorId,statusTransaction,quantity")] medicineTransaction medicineTransaction)
        {
            if (ModelState.IsValid)
            {
                db.medicineTransactions.Add(medicineTransaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.doctorId = new SelectList(db.doctors, "id", "name", medicineTransaction.doctorId);
            ViewBag.medicineId = new SelectList(db.medicines, "id", "name", medicineTransaction.medicineId);
            return View(medicineTransaction);
        }

        // GET: medicineTransactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            medicineTransaction medicineTransaction = db.medicineTransactions.Find(id);
            if (medicineTransaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.doctorId = new SelectList(db.doctors, "id", "name", medicineTransaction.doctorId);
            ViewBag.medicineId = new SelectList(db.medicines, "id", "name", medicineTransaction.medicineId);
            return View(medicineTransaction);
        }

        // POST: medicineTransactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,medicineId,doctorId,statusTransaction,quantity")] medicineTransaction medicineTransaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(medicineTransaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.doctorId = new SelectList(db.doctors, "id", "name", medicineTransaction.doctorId);
            ViewBag.medicineId = new SelectList(db.medicines, "id", "name", medicineTransaction.medicineId);
            return View(medicineTransaction);
        }

        // GET: medicineTransactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            medicineTransaction medicineTransaction = db.medicineTransactions.Find(id);
            if (medicineTransaction == null)
            {
                return HttpNotFound();
            }
            return View(medicineTransaction);
        }

        // POST: medicineTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            medicineTransaction medicineTransaction = db.medicineTransactions.Find(id);
            db.medicineTransactions.Remove(medicineTransaction);
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
