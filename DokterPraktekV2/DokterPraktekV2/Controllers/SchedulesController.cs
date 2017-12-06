using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DokterPraktekV2.Models;
using DokterPraktekV2.Services;
namespace DokterPraktekV2.Controllers
{

    public class schedulesController : Controller
    {
        private dayInServices DoctorWorkDay = new dayInServices();
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();
        private bookingListServices BookListService = new bookingListServices();
        private PatientServices patientService = new PatientServices();

        #region Get List Booking
        // GET: Schedules
        public ActionResult Index()
        {
            var book = BookListService.BookingList();
            return View(book);
        }
        #endregion

        #region Get Today Booking List
        public ActionResult TodayBook()
        {
            var book = BookListService.TodayBookingList();
            return View(book);
        }
        #endregion

        public ActionResult Select()
        {
            VM_schedules schedule = new VM_schedules();
            schedule.doctors = DoctorWorkDay.ListWorkDays(); // panggil service hari kerja dokter
            return View(schedule);
        }

        #region HttpGet Create Booking
        public ActionResult Create()
        {
            VM_schedules schedule = new VM_schedules();
            var blockDate = DateTime.Now.ToString("yyyy-MM-dd");
            schedule.doctors = DoctorWorkDay.ListWorkDays(); // panggil service hari kerja dokter
            ViewBag.blockDate = blockDate;
            return View(schedule);
        }
        #endregion

        #region HttpPost Create Booking
        [HttpPost]
        public ActionResult Create(VM_schedules data)
        {
            if (!ModelState.IsValid)
            {
                return View(data);
            }
            int docId = data.doctors[0].doctorId; // Choosen doctor id
            var checkSchedule = BookListService.CheckSchedule(data , docId); // Check schedule service
            if (checkSchedule > 0)
            {
                var checkPatient = BookListService.CheckPatient(data.name , data.phone); // check patient service
                if (checkPatient != null) 
                {
                    BookListService.CreateBooking(checkPatient.id, docId, data.dateSchedule); // Create booking service
                    return RedirectToAction("Index");
                } 
                else 
                {
                    var dataPatient = patientService.CreatePatient(data); // Create patient service
                    BookListService.CreateBooking(dataPatient.id, docId, data.dateSchedule);  // Create booking service
                } 
            }
            else
            {
                data.doctors = DoctorWorkDay.ListWorkDays(); // Get doctor working days service
                ModelState.AddModelError("dateSchedule", "Doctor is not available on that day");
                var blockDate = DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.blockDate = blockDate;
                return View(data);
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Cancel Status Booking
        public ActionResult Cancel(int id)
        {
            var bookingInfo = db.schedules.First(s => s.id == id);
            bookingInfo.bookingStatus = "Cancelled";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion

        #region Check Status Booking
        public ActionResult Check(int id)
        {
            var bookingInfo = db.schedules.First(s => s.id == id);
            bookingInfo.bookingStatus = "Completed";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion
    }
}