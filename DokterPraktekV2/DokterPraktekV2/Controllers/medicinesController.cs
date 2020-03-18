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
using PagedList;
using DokterPraktekV2.Services;

namespace DokterPraktekV2.Controllers
{
    public class medicinesController : Controller
    { 
        private DokterPraktekEntities db = new DokterPraktekEntities();
        private MedicineAttributeServices MedAttrService = new MedicineAttributeServices();
        
        // GET: medicines
        public ActionResult Index(int? page,string search,string option)
        {
            var idLog = User.Identity.GetUserId();
            var ids = db.doctors.Where(m => m.userId == idLog).First();
            var op = db.Medicines.Where(a => a.DoctorID == ids.userId).ToList();
            var medicine = db.Medicines.Include(m => m.DoctorID);
            var GetDataFromMedicineController = db.AdminMedicineControllers.FirstOrDefault();

            var IsFromCreate = "";
            if (TempData.ContainsKey("IsFromCreate"))
                IsFromCreate = TempData["IsFromCreate"].ToString();
            if(IsFromCreate == "true")
            {
                ViewBag.MessageSuccess = "Input Data Success";
                ViewBag.MessageDisplay = true;
                TempData["IsFromCreate"] = "false";
            }
            else if(IsFromCreate == "failed")
            {
                ViewBag.MessageSuccess = "Cannot Find Existing ID,Please Check Your Input";
                ViewBag.MessageDisplay = true;
                TempData["IsFromCreate"] = "false";
            }
            else
            {
                ViewBag.MessageSuccess = "";
                ViewBag.MessageDisplay = false;
                ViewBag.MsgFrom = false;
            }

            var data = db.Medicines.Select(e => new VM_Stock
            {
                id = e.ID,
                doctorId = e.DoctorID,
                nameMedicine = e.Name,
                BenefitMedicine = e.BenefitMedicine,
                UnitOfMedicine = e.UnitOfMedicine,
                MerkMedicine = e.MerkOfMedicine,
                price = e.Price,
                dateIn = e.DateIn,
                expired = e.ExpireDate,
                inStock = e.Quantity,
                outStock = e.PatientMedicines.Sum(a => a.Quantity),
                remainStock = e.Quantity - e.PatientMedicines.Sum(a => a.Quantity)
            }).ToList().Where(a=>a.doctorId == ids.userId && !string.IsNullOrEmpty(a.MerkMedicine));
            


            if (!String.IsNullOrEmpty(search))
            {
                data = data.Where(s => s.nameMedicine.ToLower().Contains(search.ToLower())).ToList();
            }
            //paged List
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            IPagedList<VM_Stock> listTrans = null;
            
            //make the default of expired date range by 30days
            DateTime estimatedDate = DateTime.Now.Date.AddDays(GetDataFromMedicineController.ExpiredDateRange == 0 ? GetDataFromMedicineController.ExpiredDateRange : 30);

            List<string> listWarna = new List<string>();
            List<string> listStatus = new List<string>();
            foreach (var item in data)
            {
                if(estimatedDate <= item.expired)
                {
                    var warna = "green";
                    var status = !string.IsNullOrEmpty(GetDataFromMedicineController.StatusSecureText) ? GetDataFromMedicineController.StatusSecureText : "SECURE";
                    listWarna.Add(warna);
                    listStatus.Add(status);
                }
                else
                {
                    var warna = "red";
                    var status = !string.IsNullOrEmpty(GetDataFromMedicineController.StatusExpiredText) ? GetDataFromMedicineController.StatusExpiredText : "EXPIRED";
                    listWarna.Add(warna);
                    listStatus.Add(status);
                }
            }
            ViewBag.warna = listWarna;
            ViewBag.status = listStatus;

            listTrans = data.ToPagedList(pageIndex, pageSize);
            return View(listTrans);
        }

        // GET: medicines/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medicine medicine = db.Medicines.Find(id);
            if (medicine == null)
            {
                return HttpNotFound();
            }
            return View(medicine);
        }

        // GET: medicines/Create
        public ActionResult Create()
        {
            var GetDataQtyMed = MedAttrService.GetListOfAttribute("Quantity");
            var GetDataQtyMedByUnit = MedAttrService.GetListOfAttribute("Unit");
            List<MedicineAttribute> CheckingID = new List<MedicineAttribute>();

            CheckingID.Add(new MedicineAttribute { AttributeValue = "Existing Product", AttributeName = "CheckID" });
            CheckingID.Add(new MedicineAttribute { AttributeValue = "New Product",AttributeName = "CheckID" });

            ViewBag.QuantityMed = GetDataQtyMed;
            ViewBag.GetDataQtyMedByUnit = GetDataQtyMedByUnit;
            ViewBag.ChekingProduct = CheckingID;
            return View();
        }
        public ActionResult MedicineAdminController()
        {
            var GetData = db.AdminMedicineControllers.FirstOrDefault();
            return View(GetData);
        }
        public ActionResult MedicineConfigurationView()
        {
            ViewBag.MessageSuccess = "";
            ViewBag.MessageDisplay = false;
            ViewBag.AllAttribute = db.MedicineAttributes;

            var IsFromCreate = "";
            if (TempData.ContainsKey("IsInsertDataFromMedConfig"))
                IsFromCreate = TempData["IsInsertDataFromMedConfig"].ToString();
            if (IsFromCreate == "true")
            {
                ViewBag.MessageSuccess = "Input Data Success";
                ViewBag.MessageDisplay = true;
                TempData["IsInsertDataFromMedConfig"] = "false";
            }
            else
            {
                ViewBag.MessageSuccess = "";
                ViewBag.MessageDisplay = false;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MedicineAdminController(AdminMedicineController adminViewMedicine)
        {
            try
            {
                var GetData = db.AdminMedicineControllers.FirstOrDefault();
                if (GetData == null)
                {
                    db.AdminMedicineControllers.Add(adminViewMedicine);
                }
                else
                {
                    GetData = db.AdminMedicineControllers.FirstOrDefault(e => e.ID == adminViewMedicine.ID);
                    GetData.StatusExpiredText = adminViewMedicine.StatusExpiredText;
                    GetData.StatusSecureText = adminViewMedicine.StatusSecureText;
                    GetData.ExpiredDateRange = adminViewMedicine.ExpiredDateRange;
                }
                ViewBag.MessageSuccess = "Input Data Success";
                ViewBag.MessageDisplay = true;
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MedicineConfigurationView(MedicineAttribute medAttr)
        {
            db.MedicineAttributes.Add(medAttr);
            db.SaveChanges();
            TempData["IsInsertDataFromMedConfig"] = "true";
            return RedirectToAction("MedicineConfigurationView");
            //return View();
        }

        public ActionResult DeleteMedConf(int id)
        {
            var GetData = (from item in db.MedicineAttributes
                       where item.ID == id
                       select item
                       ).FirstOrDefault();

            db.MedicineAttributes.Remove(GetData);
            db.SaveChanges();
            return RedirectToAction("MedicineConfigurationView");
        }


        // POST: medicines/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        //public ActionResult AdminViewMedicine(AdminViewMedicine adminViewMedicine)
        //{
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,name,price,quantity,dateIn,ExpireDate,UnitOfMedicine,BenefitMedicine,MerkOfMedicine,QuantityOfMedicine")] Medicine medicine,[Bind(Include ="id,medicineId,statusTransaction,quantity")] MedicineTransaction medtrans,string NewProd)
        {
            var idLog = User.Identity.GetUserId();
            var ids = db.doctors.Where(m => m.userId == idLog).First();
            medicine.DateIn = DateTime.Today;
            ViewBag.getData = db.MedicineTransactions;

            var DataDB = db.Medicines.Where(e => e.DoctorID == ids.userId);
            bool result = false;
            var GetID = 0;
            if (NewProd.Contains("New"))
            {
                result = false;
            }
            else
            {
                foreach (var item in DataDB)
                {
                    if (item.ID == medicine.ID)
                    {
                        result = true;
                        break;
                    }
                }
            }
            

            if (ModelState.IsValid)
            {
                if (result)
                {

                    //medtrans.DoctorID = ids.userId;
                    //medtrans.TransactionStatus = true;
                    //medtrans.TransactionDate = DateTime.Now;
                    //db.MedicineTransactions.Add(medtrans);
                    

                    var dateIn = db.Medicines.Where(m => m.ID == medicine.ID).First();
                    var getMedTrans = db.MedicineTransactions.Where(e => e.MedicineID == medicine.ID).First();

                    dateIn.DoctorID = ids.userId;
                    dateIn.Name = medicine.Name;
                    dateIn.Price = medicine.Price;
                    dateIn.MerkOfMedicine = medicine.MerkOfMedicine;
                    dateIn.UnitOfMedicine = medicine.UnitOfMedicine;
                    dateIn.BenefitMedicine = medicine.BenefitMedicine;
                    dateIn.Quantity = medicine.Quantity + dateIn.Quantity;
                    dateIn.QuantityOfMedicine = medicine.QuantityOfMedicine;
                    dateIn.ExpireDate = medicine.ExpireDate;


                    getMedTrans.Quantity = dateIn.Quantity;
                    db.Entry(getMedTrans).State = EntityState.Modified;

                    db.Entry(dateIn).State = EntityState.Modified;

                    db.SaveChanges();

                    
                    //db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else if (!result && medicine.ID != 0)
                {
                    TempData["IsFromCreate"] = "failed";
                    return RedirectToAction("Index");
                }
                else
                {
                    medtrans.DoctorID = ids.userId;
                    medtrans.TransactionStatus = true;
                    medtrans.TransactionDate = DateTime.Now;
                    db.MedicineTransactions.Add(medtrans);
                    medicine.DoctorID = ids.userId;
                    db.Medicines.Add(medicine);
                    db.SaveChanges();
                    TempData["IsFromCreate"] = "true";
                    return RedirectToAction("Index");
                }
                
                TempData["IsFromCreate"] = "true";
                return RedirectToAction("Index");
            }
            ViewBag.doctorId = new SelectList(db.doctors, "id", "name", medicine.DoctorID);
            return View(medicine);
        }

        // GET: medicines/Edit/5
        public ActionResult Edit(int? id)
        {
            var idLog = User.Identity.GetUserId();
            var ids = db.doctors.Where(m => m.userId == idLog).First();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medicine medicine = db.Medicines.Find(id);
            var getID = db.Medicines.Find(id).DoctorID;
            ViewBag.getID = getID;
            if (medicine == null)
            {
                return HttpNotFound();
            }
            ViewBag.doctorId = new SelectList(db.doctors, "id", "name", medicine.DoctorID);

            var GetDataQtyMed = MedAttrService.GetListOfAttribute("Quantity");
            var GetDataQtyMedByUnit = MedAttrService.GetListOfAttribute("Unit");
            ViewBag.QuantityMed = GetDataQtyMed;
            ViewBag.GetDataQtyMedByUnit = GetDataQtyMedByUnit;

            return View(medicine);
        }

        // POST: medicines/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Medicine medicine)
        {
            var idLog = User.Identity.GetUserId();
            var ids = db.doctors.Where(m => m.userId == idLog).First();
            var dateIn = db.Medicines.Where(m => m.ID == medicine.ID).First();

            if (ModelState.IsValid)
            {
                dateIn.DoctorID = ids.userId;
                dateIn.Name = medicine.Name;
                dateIn.Price = medicine.Price;
                dateIn.MerkOfMedicine = medicine.MerkOfMedicine;
                dateIn.UnitOfMedicine = medicine.UnitOfMedicine;
                dateIn.BenefitMedicine = medicine.BenefitMedicine;
                dateIn.Quantity = medicine.Quantity;
                dateIn.QuantityOfMedicine = medicine.QuantityOfMedicine;
                dateIn.ExpireDate = medicine.ExpireDate;
                db.Entry(dateIn).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.doctorId = new SelectList(db.doctors, "id", "name", medicine.DoctorID);
            return View(medicine);
        }

        // GET: medicines/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medicine medicine = db.Medicines.Find(id);
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
            Medicine medicine = db.Medicines.Find(id);
            db.Medicines.Remove(medicine);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Stock()
        {
           
            var idLog = User.Identity.GetUserId();
            var ids = db.doctors.Where(m => m.userId == idLog).First();
            var op = db.Medicines.Where(a => Convert.ToInt32(a.DoctorID) == ids.id).ToList();

            var data = db.Medicines.Select(e => new VM_Stock
            {
                id = e.ID,
                doctorId = e.DoctorID,
                nameMedicine = e.Name,
                price = e.Price,
                dateIn = e.DateIn,
                expired = e.ExpireDate,
                inStock = e.Quantity,
                outStock = e.PatientMedicines.Sum(a => a.Quantity),
                remainStock = e.Quantity - e.PatientMedicines.Sum(a=>a.Quantity)
            }).ToList();
            ViewBag.Data = data;
            
            return View(data);
        }

        public ActionResult DataTable()
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
            List<VM_Stock> medicines = new List<VM_Stock>();
            DateTime estimatedDate = DateTime.Now.Date.AddDays(30);
            medicines = db.Medicines
                .Where(e => e.DoctorID == dataDoctor.userId && estimatedDate <= e.ExpireDate)//estimatedDate <= item.expired || item.expired <= DateTime.Now.Date
                .Select(e => new VM_Stock
                {
                    id = e.ID,
                    doctorId = e.DoctorID,
                    nameMedicine = e.Name,
                    price = e.Price,
                    dateIn = e.DateIn,
                    expired = e.ExpireDate,
                    inStock = e.Quantity,
                    outStock = e.PatientMedicines.Sum(a => a.Quantity),
                    remainStock = e.Quantity - e.PatientMedicines.Sum(a => a.Quantity)
                }).Where(a => a.remainStock > 0 || a.remainStock == null).ToList();
            int total = medicines.Count();

            //jika searchbox tidak kosong ekseskusi dibawah
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                medicines = medicines
                    .Where(x => x.nameMedicine.ToLower().Contains(searchValue) && x.doctorId == dataDoctor.userId)
                    .ToList();
            }
            int totalFilter = medicines.Count();

            medicines = medicines.Skip(start).Take(length).ToList();

            return Json(new
            {
                recordsTotal = total,
                recordsFiltered = totalFilter,
                data = medicines,
                draw = Request["draw"]
            }, JsonRequestBehavior.AllowGet);
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
