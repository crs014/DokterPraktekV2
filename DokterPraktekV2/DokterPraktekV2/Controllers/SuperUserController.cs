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
        DokterPraktekEntities1 db = new DokterPraktekEntities1();
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

        public ActionResult DoctorDetail(int id)
        {
            doctor data = db.doctors.FirstOrDefault(e => e.id == id);
            List<workDay> work = db.workDays.Where(d => d.working == true && d.doctorId == id).ToList(); 
            ViewBag.work = work;
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