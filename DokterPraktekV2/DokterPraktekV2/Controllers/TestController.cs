using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DokterPraktekV2;
using DokterPraktekV2.Models;
using Microsoft.Ajax.Utilities;

namespace DokterPraktekV2.Controllers
{
    public class TestController : Controller
    {

        DokterPraktekEntities1 db = new DokterPraktekEntities1();
        // GET: Test
        public ActionResult Index()
        {
         
            var data = db.medicines.Select(e => new VM_test {
                id = e.id,
                name = e.name,
                inStock = e.quantity,
                outStock = e.patientMedicines.Sum(a => a.quantity)
            }).ToList();

            ViewBag.data = data;
            return View();
        }

        public ActionResult Index2()
        {
            var tr = new history();
            var data = db.histories.Where(a => a.doctorId == 3).DistinctBy(a => a.patientId).Select(e => new VM_test2
            {
                patientId = e.patientId,
                patientName = e.patient.name,

            }).ToList();

            ViewBag.data = data;
            return View();
        }
    }
}