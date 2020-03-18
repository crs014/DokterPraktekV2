using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DokterPraktekV2;
using DokterPraktekV2.Models;
using Microsoft.Ajax.Utilities;
using PagedList;

namespace DokterPraktekV2.Controllers
{
    [Authorize(Roles = "admin")]
    public class TransactionController : Controller
    {
        DokterPraktekEntities db = new DokterPraktekEntities();

        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            var DataDoctor = db.doctors;
            List<doctor> doc = new List<doctor>();
            foreach (var item in DataDoctor)
            {
                doc.Add(item);
            }
            var data = db.MedicalHistories.DistinctBy(z => z.ID).Select(e => new VM_transaction
            {
                alreadyPay = e.Payments.Sum(a => a.Amount),
                DoctorName = doc.FirstOrDefault(asd=>asd.userId == e.DoctorID).name,
                patientId = e.PatientID,
                patientName = e.Patient.Name,
                amount = e.CheckUpPrice + e.PatientMedicines.Sum(b => b.Medicine.Price * b.Quantity),
                dateHistory = e.CheckUpDate,
                historyId = e.ID
            }).OrderByDescending(e => e.dateHistory).ToList();

            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                int n;
                bool isNumeric = int.TryParse(searchString, out n);
                if(isNumeric == true)
                {
                    data = data.Where(e => e.historyId == Convert.ToInt32(searchString) || e.patientName.Contains(searchString)).ToList();
                }
                else
                {
                    data = data.Where(e => e.patientName.Contains(searchString)).ToList();
                }
            }

            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(data.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult Nota(int id)
        {
            var DataDoctor = db.doctors;
            VM_transaction data = db.MedicalHistories.DistinctBy(z => z.ID).Select(e => new VM_transaction
            {
                alreadyPay = e.Payments.Sum(a => a.Amount),
                DoctorName = DataDoctor.FirstOrDefault(d=>d.userId == e.DoctorID).name,
                patientId = e.PatientID,
                patientName = e.Patient.Name,
                amount = e.CheckUpPrice + e.PatientMedicines.Sum(b => b.Medicine.Price * b.Quantity),
                dateHistory = e.CheckUpDate,
                historyId = e.ID
            }).FirstOrDefault(a => a.historyId == id);
            ViewBag.dataMedicine = db.MedicalHistories.FirstOrDefault(e => e.ID == id).PatientMedicines.ToList();
            ViewBag.data = data;
            return View();

        }


        [NonAction]
        public Nullable<decimal> totalAll(int id)
        {

            Nullable<decimal> totalPay = db.Payments.Where(e => e.MedicalHistoryID == id).Select(a => new VM_totalTrans
            {
                alreadyPay = a.Amount
            }).Sum(a => a.alreadyPay);

            Nullable<decimal> totalMed = db.PatientMedicines.Where(e => e.MedicalHistoryID == id).Select(a => new VM_totalTrans
            {
                alreadyPay = a.Medicine.Price * a.Quantity
            }).Sum(e => e.alreadyPay);

            decimal priceChk = db.MedicalHistories.FirstOrDefault(e => e.ID == id).CheckUpPrice;
            if (totalPay == null)
            {
                totalPay = 0;
            }
            if (totalMed == null)
            {
                totalMed = 0;
            }

            var subtotal = totalMed + priceChk;
            var grandTotalPaid = subtotal - totalPay;
            return grandTotalPaid;
        }

        [HttpPost]
        public ActionResult Create(int id,VM_transactionInput inputz)
        {
            var grandTotalPaid = totalAll(id);
            if (grandTotalPaid >= inputz.amount)
            {

                Payment pay = new Payment();
                pay.MedicalHistoryID = id;
                pay.Amount = inputz.amount;
                db.Payments.Add(pay);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
