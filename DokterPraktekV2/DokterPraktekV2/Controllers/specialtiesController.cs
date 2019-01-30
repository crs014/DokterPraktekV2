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
        private DokterPraktekEntities db = new DokterPraktekEntities();
        private SpecialtyService SpecialtyService = new SpecialtyService();

        #region Add Specialties
        [Authorize(Roles = "doctor")]
        // GET: specialties/Create
        public ActionResult Add()
        {
            VM_Specialties special = new VM_Specialties();
            var auth_id = User.Identity.GetUserId();
            var docId = db.doctors.Where(s => s.userId == auth_id).Select(s => s.userId).FirstOrDefault();
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
            var docId = db.doctors.Where(s => s.userId == auth_id).Select(s => s.userId).FirstOrDefault();
            var CheckExist = (from s in db.DoctorSpecialists
                              where s.DoctorID == docId.ToString() && s.SpecialtyID == special.SelectedId
                              select s.ID).Count();
            if(CheckExist > 0)
            {
                special.ListSpecialties = SpecialtyService.GetAllSpecialty();
                special.ListDoctorSpecialties = SpecialtyService.GetAllSpecialtyByDocId(docId);
                ModelState.AddModelError("SelectedId","Selected specialty is already added");
                return View(special);
            }
            else
            {
                var AddSpecialty = new DoctorSpecialist()
                {
                    AspNetUser = db.AspNetUsers.FirstOrDefault(e => e.Id == auth_id),
                    DoctorID = docId.ToString(),
                    SpecialtyID = special.SelectedId
                };
                db.DoctorSpecialists.Add(AddSpecialty);
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
            var CheckExist = (from item in db.Specialists
                              where item.SpecialistName == data.specialty
                              select item.ID).Count();
            if (CheckExist > 0)
            {
                ModelState.AddModelError("specialty", "Specialty is already exist");
                return RedirectToAction("Add");
            }
            else
            {
                var NewSpecialty = new Specialist()
                {
                    SpecialistName = data.specialty
                };
                db.Specialists.Add(NewSpecialty);
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
            var docId = db.doctors.Where(s => s.userId == auth_id).Select(s => s.userId).FirstOrDefault();
            DoctorSpecialist ds = (from item in db.DoctorSpecialists
                                   where item.DoctorID == docId.ToString() && item.SpecialtyID == id
                                   select item).FirstOrDefault();
            //var DataSpecialty = (from data in db.doctorSpecialists
            //                     where data.doctorId == docId && data.specialtyId == id
            //                     select data).FirstOrDefault();
            db.DoctorSpecialists.Remove(ds);
            db.SaveChanges();
            return RedirectToAction("Add");
        }
        #endregion
    }
}
