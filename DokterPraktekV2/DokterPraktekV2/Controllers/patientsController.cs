using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DokterPraktekV2.Services;
using DokterPraktekV2.Models;
using PagedList;
using System.Net;

namespace DokterPraktekV2.Controllers
{
    
    public class PatientsController : Controller
    {

        private PatientServices patientService = new PatientServices();

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


        public ActionResult Details(int? id, int ? page)
        {
            var data = patientService.allPatientHistory(id);
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

        public ActionResult detailsHistory(int ? id)
        {
            var data = patientService.detailHistory(id);
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
