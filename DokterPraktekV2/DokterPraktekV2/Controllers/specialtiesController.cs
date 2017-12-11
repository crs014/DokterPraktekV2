using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DokterPraktekV2.Models;
using Microsoft.AspNet.Identity;

namespace DokterPraktekV2.Controllers
{
    public class specialtiesController : Controller
    {
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();
        // GET: specialties
        public ActionResult Index()
        {
            return View();
        }

        // GET: specialties/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        
        // GET: specialties/Create
        public ActionResult Create()
        {
            VM_Specialties special = new VM_Specialties();
            special.ListSpecialties = (from data in db.specialists
                                       select new SpecialtyData
                                       {
                                           id = data.id,
                                           specialty = data.specialty
                                       }).ToList();
            ViewBag.Specials = new MultiSelectList(special.ListSpecialties, "id", "specialty");
            return View(special);
        }

        // POST: specialties/Create
        [HttpPost]
        public ActionResult Create(VM_Specialties item)
        {
            var authId = User.Identity.GetUserId();
            var docId = db.doctors.Where(s => s.userId == authId).Select(s => s.id).FirstOrDefault();
            return View();
        }

        // GET: specialties/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: specialties/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: specialties/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: specialties/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
