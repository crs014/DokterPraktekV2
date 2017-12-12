using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DokterPraktekV2.Models;
using Microsoft.AspNet.Identity;
using DokterPraktekV2.Services;
namespace DokterPraktekV2.Controllers
{
    public class specialtiesController : Controller
    {
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();
        private SpecialtyService SpecialtyService = new SpecialtyService();

        #region Add Specialties
        [Authorize(Roles = "doctor")]
        // GET: specialties/Create
        public ActionResult Add()
        {
            VM_Specialties special = new VM_Specialties();
            var auth_id = User.Identity.GetUserId();
            var docId = db.doctors.Where(s => s.userId == auth_id).Select(s => s.id).FirstOrDefault();
            special.ListSpecialties = SpecialtyService.GetAllSpecialty();
            special.ListDoctorSpecialties = SpecialtyService.GetAllSpecialtyByDocId(docId);
            return View(special);
        }
        #endregion

        #region Add New Specialties to Doctor
        [HttpPost]
        public ActionResult Add(VM_Specialties special)
        {
            var auth_id = User.Identity.GetUserId();
            var docId = db.doctors.Where(s => s.userId == auth_id).Select(s => s.id).FirstOrDefault();
            var CheckExist = (from s in db.doctorSpecialists
                              where s.doctorId == docId && s.specialtyId == special.SelectedId
                              select s.id).Count();
            if(CheckExist > 0)
            {
                special.ListSpecialties = SpecialtyService.GetAllSpecialty();
                special.ListDoctorSpecialties = SpecialtyService.GetAllSpecialtyByDocId(docId);
                ModelState.AddModelError("SelectedId","Selected specialty is already added");
                return View(special);
            }
            else
            {
                var AddSpecialty = new doctorSpecialist()
                {
                    doctorId = docId,
                    specialtyId = special.SelectedId
                };
                db.doctorSpecialists.Add(AddSpecialty);
                db.SaveChanges();
            }
            special.ListSpecialties = SpecialtyService.GetAllSpecialty();
            special.ListDoctorSpecialties = SpecialtyService.GetAllSpecialtyByDocId(docId);
            return View(special);
        }
        #endregion

        #region Create New Specialty
        [HttpPost]
        public ActionResult CreateNew(SpecialtyData data)
        {
            var CheckExist = (from item in db.specialists
                              where item.specialty == data.specialty
                              select item.id).Count();
            if (CheckExist > 0)
            {
                ModelState.AddModelError("specialty", "Specialty is already exist");
                return RedirectToAction("Add");
            }
            else
            {
                var NewSpecialty = new specialist()
                {
                    specialty = data.specialty
                };
                db.specialists.Add(NewSpecialty);
                db.SaveChanges();
            }
            return RedirectToAction("Add");
        }

        #endregion

        #region Remove Specialties
        // POST: specialties/Delete/5
        public ActionResult Delete(int id)
        {
            VM_Specialties special = new VM_Specialties();
            var auth_id = User.Identity.GetUserId();
            var docId = db.doctors.Where(s => s.userId == auth_id).Select(s => s.id).FirstOrDefault();
            doctorSpecialist ds = (from item in db.doctorSpecialists
                                   where item.doctorId == docId && item.specialtyId == id
                                   select item).FirstOrDefault();
            //var DataSpecialty = (from data in db.doctorSpecialists
            //                     where data.doctorId == docId && data.specialtyId == id
            //                     select data).FirstOrDefault();
            db.doctorSpecialists.Remove(ds);
            db.SaveChanges();
            return RedirectToAction("Add");
        }
        #endregion
    }
}
