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
        DokterPraktekEntities1 db = new DokterPraktekEntities1();

        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            var data = db.histories.DistinctBy(z => z.id).Select(e => new VM_transaction
            {
                alreadyPay = e.payments.Sum(a => a.amount),
                patientId = e.patientId,
                patientName = e.patient.name,
                amount = e.checkupPrice + e.patientMedicines.Sum(b => b.medicine.price * b.quantity),
                dateHistory = e.checkupDate,
                historyId = e.id
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
            VM_transaction data = db.histories.DistinctBy(z => z.id).Select(e => new VM_transaction
            {
                alreadyPay = e.payments.Sum(a => a.amount),
                patientId = e.patientId,
                patientName = e.patient.name,
                amount = e.checkupPrice + e.patientMedicines.Sum(b => b.medicine.price * b.quantity),
                dateHistory = e.checkupDate,
                historyId = e.id
            }).FirstOrDefault(a => a.historyId == id);
            ViewBag.dataMedicine = db.histories.FirstOrDefault(e => e.id == id).patientMedicines.ToList();
            ViewBag.data = data;
            return View();

        }


        [NonAction]
        public Nullable<decimal> totalAll(int id)
        {

            Nullable<decimal> totalPay = db.payments.Where(e => e.historyId == id).Select(a => new VM_totalTrans
            {
                alreadyPay = a.amount
            }).Sum(a => a.alreadyPay);

            Nullable<decimal> totalMed = db.patientMedicines.Where(e => e.historyId == id).Select(a => new VM_totalTrans
            {
                alreadyPay = a.medicine.price * a.quantity
            }).Sum(e => e.alreadyPay);

            decimal priceChk = db.histories.FirstOrDefault(e => e.id == id).checkupPrice;
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

                payment pay = new payment();
                pay.historyId = id;
                pay.amount = inputz.amount;
                db.payments.Add(pay);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
