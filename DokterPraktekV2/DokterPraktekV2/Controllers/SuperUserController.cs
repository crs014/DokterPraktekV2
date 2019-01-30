using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DokterPraktekV2;
using PagedList;

namespace DokterPraktekV2.Controllers
{
    [Authorize(Roles = "superuser")]
    public class SuperUserController : Controller
    {
        DokterPraktekEntities db = new DokterPraktekEntities();
        // GET: SuperUser
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Doctor(string currentFilter, string searchString, int? page)
        {

            /*call all data from service*/
            List<doctor> data = db.doctors.ToList();

            /*search data from name patient*/
            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                data = db.doctors.Where(e => e.name.Contains(searchString)).ToList();
            }

            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult DoctorDetail(string id)
        {
            doctor data = db.doctors.FirstOrDefault(e => e.userId == id);
            List<WorkSchedule> work = db.WorkSchedules.Where(d => d.IsAvailable == true && d.DoctorID == id.ToString()).ToList();
            List<DoctorSpecialist> speciality = db.DoctorSpecialists.Where(e => e.DoctorID == id).ToList();
            List<Specialist> specialistDoc = new List<Specialist>();
            foreach (var item in speciality)
            {
                var abc = db.Specialists.Where(e => e.ID == item.SpecialtyID).FirstOrDefault();
                specialistDoc.Add(abc);
            }
            ViewBag.SpecialistDoc = specialistDoc;
            ViewBag.work = work;
            ViewBag.SpecialityDetail = db.Specialists;
            return View(data);
        }

        public ActionResult Admin(string currentFilter, string searchString, int? page)
        {

            /*call all data from service*/
            List<admin> data = db.admins.ToList();

            /*search data from name patient*/
            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                data = db.admins.Where(e => e.name.Contains(searchString)).ToList();
            }

            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(data.ToPagedList(pageNumber, pageSize));
        }
    }
}