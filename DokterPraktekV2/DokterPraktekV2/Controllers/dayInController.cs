using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DokterPraktekV2.Models;
using DokterPraktekV2.Services;
using Microsoft.AspNet.Identity;

namespace DokterPraktekV2.Controllers
{
    public class dayInController : Controller
    {
        private DokterPraktekEntities db = new DokterPraktekEntities();
        private dayInServices serv = new dayInServices();

        [Authorize(Roles = "doctor")]
        // GET: WorkDays/Create
        public ActionResult Create()
        {
            var auth_id = User.Identity.GetUserId();
            var id = db.doctors.Where(s => s.userId == auth_id).Select(s => s.userId).FirstOrDefault();
            VM_dayIn work = new VM_dayIn();
            work.dayIn = serv.DoctorDayIn(id);
            work.docInfo = serv.DoctorNames(id);
            //work.docId = Convert.ToInt32(id);
            return View(work);
        }

        // POST: WorkDays/Create
        [HttpPost]
        public ActionResult Create(VM_dayIn data)
        {
            foreach (var item in data.dayIn)
            {
                var docInfo = db.WorkSchedules.First(s => s.ID == item.dayId);
                if(item.IsSelected == true)
                {
                    docInfo.IsAvailable = true;
                    db.SaveChanges();
                }
                else
                {
                    docInfo.IsAvailable = false;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index","doctors");
        }
    }
}
