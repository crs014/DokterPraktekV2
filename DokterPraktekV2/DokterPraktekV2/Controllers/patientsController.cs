﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DokterPraktekV2.Services;
using DokterPraktekV2.Models;
using PagedList;
using System.Net;
using Microsoft.AspNet.Identity;
using DokterPraktekV2;

namespace DokterPraktekV2.Controllers
{
    
    public class PatientsController : Controller
    {

        private PatientServices patientService = new PatientServices();
        private DokterPraktekEntities db = new DokterPraktekEntities();
        private PhotoService photoService = new PhotoService();
        [Authorize(Roles = "doctor")]
        public  ActionResult Index(string currentFilter, string searchString, int? page)
        {
            var userID = User.Identity.GetUserId();
            doctor dataDoctor = db.doctors.FirstOrDefault(e => e.userId == userID);

            /*call all data from service*/
            var data = patientService.allDoctorPatient(dataDoctor.userId);
            /*search data from name patient*/
            if (searchString != null){page = 1;}
            else{searchString = currentFilter;}
            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                data = patientService.searchPatientFromNameAndIdDoctor(searchString,dataDoctor.id);
            }

            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(data.ToPagedList(pageNumber,pageSize));
        }

        [Authorize(Roles = "superuser,admin")]
        public ActionResult allPatient(string currentFilter, string searchString, int? page)
        {
            /*call all data from service*/
            var data = db.Patients.ToList();
            /*search data from name patient*/
            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                data = db.Patients.Where(e => e.Name.Contains(searchString)).ToList();
            }
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "doctor")]
        public ActionResult Details(int?id, int ? page)
        {
            var userID = User.Identity.GetUserId();
            doctor dataDoctor = db.doctors.FirstOrDefault(e => e.userId == userID);
            string mime;
            string convertedImage = photoService.LoadImage(id, out mime);
            var data = patientService.allPatientHistory(id, dataDoctor.userId);
            int pageNumber = (page ?? 1);
            int pageSize = 7;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (data == null)
            {
                return HttpNotFound();
            }
            ViewBag.tipeImage = mime;
            ViewBag.stringUrl = convertedImage;
            ViewBag.detailPatient = patientService.patientDetail(id);
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "doctor")]
        public ActionResult detailsHistory(int ? id)
        {
            var userID = User.Identity.GetUserId();
            doctor dataDoctor = db.doctors.FirstOrDefault(e => e.userId == userID);

            var data = patientService.detailHistory(id, dataDoctor.userId);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (data == null)
            {
                return HttpNotFound();
            }
            ViewBag.Total = patientService.totalMedicinePatient(id);
            ViewBag.Medicines = patientService.getAllMedicinePatient(id);
            return View(data);
        }

    }
}
