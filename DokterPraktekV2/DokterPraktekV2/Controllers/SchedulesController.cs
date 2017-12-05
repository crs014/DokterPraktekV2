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
            var dayChoosen = data.dateSchedule.DayOfWeek.ToString(); // Hari yang dipilih
            int docId = data.doctors[0].doctorId; // Id dokter
            var checkSchedule = (from works in db.workDays
                                 where works.doctorId == docId && works.dayIn == dayChoosen && works.working == true
                                 select works).Count();
            if (checkSchedule > 0) // Check jika hari yang dipilih sesuai dengan jadwal dokter
            {
                var checkPasien = db.patients.Where(s => s.name == data.name && s.phone == data.phone).Select(s => s.id);
                var checkData = checkPasien.Count(); // cek data pasien sudah ada atau belum
                if (checkData > 0) // Check jika ada maka masukkan data kedalam schedule
                {
                    var dataSchedule = new schedule()
                    {
                        patientId = (from person in db.patients
                                     where person.name == data.name && person.phone == data.phone
                                     select person.id).First(),
                        doctorId = docId,
                        dateSchedule = data.dateSchedule,
                        bookingStatus = "Booking"
                    };
                    db.schedules.Add(dataSchedule);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                } // End if
                else // jika belum , buat data pasien dan buat data schedule
                {
                    var dataPatient = new patient()
                    {
                        name = data.name,
                        homeAddress = data.homeAddress,
                        phone = data.phone,
                        gender = data.gender
                    };

                    db.patients.Add(dataPatient);
                    db.SaveChanges();

                    var dataSchedule = new schedule()
                    {
                        doctorId = docId,
                        patientId = dataPatient.id,
                        dateSchedule = data.dateSchedule,
                        bookingStatus = "Booking"
                    };
                    db.schedules.Add(dataSchedule);
                    db.SaveChanges();
                } // End Else
            } // End if checkschedule
            else
            {
                data.doctors = DoctorWorkDay.ListWorkDays();
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