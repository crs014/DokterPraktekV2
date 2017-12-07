﻿using System;
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

        #region Select Doctor
        public ActionResult Select()
        {
            VM_schedules schedule = new VM_schedules();
            schedule.doctors = DoctorWorkDay.ListWorkDays(); // panggil service hari kerja dokter
            return View(schedule);
        }
        #endregion

        #region HttpGet Create Booking
        public ActionResult Create(int id)
        {
            VM_schedules schedule = new VM_schedules();
            schedule.doctors = new List<VM_doctorList>()
            {
                new VM_doctorList { doctorId = id }
            };
            var blockDate = DateTime.Now.ToString("yyyy-MM-dd");
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
                    var dataBooking = BookListService.CreateBooking(checkPatient.id, docId, data.dateSchedule); // Create booking service
                    TempData["id"] = dataBooking;
                    return RedirectToAction("formResult");
                } 
                else 
                {
                    var dataPatient = patientService.CreatePatient(data); // Create patient service
                    var dataBooking = BookListService.CreateBooking(dataPatient.id, docId, data.dateSchedule);  // Create booking service
                    TempData["id"] = dataBooking;
                    return RedirectToAction("formResult");
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

        #region Sign get
        public ActionResult Sign(int id)
        {
            var model = id;
            return View(model);
        }


        #endregion

        #region SignIn HttpGet

        public ActionResult SignIn(int id)
        {
            VM_SignIn sign = new VM_SignIn();
            sign.doctorId = id;
            var blockDate = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.blockDate = blockDate;
            return View(sign);
        }

        #endregion

        #region SignIn HttpPost
        [HttpPost]
        public ActionResult SignIn(VM_SignIn sign)
        {
            if (!ModelState.IsValid)
            {
                return View(sign);
            }
            var checkPatient = BookListService.CheckPatientById(sign.PatientNumber); // check patient service
            if(checkPatient != null)
            {
                var dataBooking = BookListService.CreateBooking(sign.PatientNumber, sign.doctorId, sign.dateSchedule); // Create booking service
                TempData["id"] = dataBooking;
                return RedirectToAction("formResult");
            }
            else
            {
                ModelState.AddModelError("PatientNumber", "Your Patient Number is wrong");
                return View(sign);
            }
        }

        #endregion

        #region FormResult HttpGet

        public ActionResult formResult()
        {
            int id = (int)TempData["id"];
            var bookingResult = BookListService.BookingListById(id);
            return View(bookingResult); 
        }

        #endregion
    }
}