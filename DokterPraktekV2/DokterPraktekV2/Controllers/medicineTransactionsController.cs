using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DokterPraktekV2;
using Microsoft.AspNet.Identity;
using PagedList;

namespace DokterPraktekV2.Controllers
{
    public class medicineTransactionsController : Controller
    {
        private DokterPraktekEntities db = new DokterPraktekEntities();

        // GET: medicineTransactions
        public ActionResult Index(int? page)
        {
            var idLog = User.Identity.GetUserId();
            var ids = db.doctors.Where(m => m.userId == idLog).First();
            var op = db.MedicineTransactions.Where(a => a.DoctorID == ids.userId).ToList();
            var b = db.MedicineTransactions.Include(m => m.MedicineID);
            ViewBag.a = b;

            //paged list
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<MedicineTransaction> listTrans = null;
            
            var query = from s in op
                        orderby s.TransactionStatus == true
                        select op;
            ViewBag.query = query.ToList();

            var getMed = db.Medicines.Include(a => a.MedicineTransactions).Include(a => a.MedicineTransactions);
            var medicineTransaction = db.MedicineTransactions.Include(m => m.Medicine).Include(m => m.Medicine);
            listTrans = op.ToPagedList(pageIndex, pageSize); //goto pagedlist and return it

            return View(listTrans);
        }

        // GET: medicineTransactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicineTransaction medicineTransaction = db.MedicineTransactions.Find(id);
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
            ViewBag.medicineId = new SelectList(db.Medicines, "id", "name");
            return View();
        }

        // POST: medicineTransactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,medicineId,doctorId,statusTransaction,quantity")] MedicineTransaction medicineTransaction)
        {
            if (ModelState.IsValid)
            {
                db.MedicineTransactions.Add(medicineTransaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.doctorId = new SelectList(db.doctors, "id", "name", medicineTransaction.DoctorID);
            ViewBag.medicineId = new SelectList(db.Medicines, "id", "name", medicineTransaction.MedicineID);
            return View(medicineTransaction);
        }

        // GET: medicineTransactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicineTransaction medicineTransaction = db.MedicineTransactions.Find(id);
            if (medicineTransaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.doctorId = new SelectList(db.doctors, "id", "name", medicineTransaction.DoctorID);
            ViewBag.medicineId = new SelectList(db.Medicines, "id", "name", medicineTransaction.MedicineID);
            return View(medicineTransaction);
        }

        // POST: medicineTransactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,medicineId,doctorId,statusTransaction,quantity")] MedicineTransaction medicineTransaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(medicineTransaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.doctorId = new SelectList(db.doctors, "id", "name", medicineTransaction.DoctorID);
            ViewBag.medicineId = new SelectList(db.Medicines, "id", "name", medicineTransaction.MedicineID);
            return View(medicineTransaction);
        }

        // GET: medicineTransactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicineTransaction medicineTransaction = db.MedicineTransactions.Find(id);
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
            MedicineTransaction medicineTransaction = db.MedicineTransactions.Find(id);
            db.MedicineTransactions.Remove(medicineTransaction);
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
