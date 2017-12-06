using System;
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
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();

        [Authorize(Roles = "doctor")]
        public  ActionResult Index(string currentFilter, string searchString, int? page)
        {
            /*call all data from service*/
            var data = patientService.allPatient();
            /*search data from name patient*/
            if (searchString != null){page = 1;}
            else{searchString = currentFilter;}
            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                data = patientService.searchPatientFromName(searchString);
            }

            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(data.ToPagedList(pageNumber,pageSize));
        }

        [Authorize(Roles = "doctor")]
        public ActionResult Details(int?id, int ? page)
        {
            var userID = User.Identity.GetUserId();
            doctor dataDoctor = db.doctors.FirstOrDefault(e => e.userId == userID);

            var data = patientService.allPatientHistory(id, dataDoctor.id);
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
            ViewBag.detailPatient = patientService.patientDetail(id);
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "doctor")]
        public ActionResult detailsHistory(int ? id)
        {
            var userID = User.Identity.GetUserId();
            doctor dataDoctor = db.doctors.FirstOrDefault(e => e.userId == userID);

            var data = patientService.detailHistory(id, dataDoctor.id);
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
