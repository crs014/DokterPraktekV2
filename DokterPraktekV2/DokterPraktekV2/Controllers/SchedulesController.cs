using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DokterPraktekV2.Models;
using DokterPraktekV2.Services;
using PagedList;
namespace DokterPraktekV2.Controllers
{

    public class schedulesController : Controller
    {
        private dayInServices DoctorWorkDay = new dayInServices();
        private DokterPraktekEntities db = new DokterPraktekEntities();
        private bookingListServices BookListService = new bookingListServices();
        private PatientServices patientService = new PatientServices();
        private PhotoService photoService = new PhotoService();

        #region Get List Booking
        // GET: Schedules
        [Authorize(Roles = "admin, superuser")]
        public ActionResult Index(string searchString , string currentFilter , int? page)
        {
            var book = BookListService.BookingList();
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            if(!String.IsNullOrEmpty(searchString))
            {
                book = BookListService.SearchBookingList(searchString);
            }
            int pageNumber = (page ?? 1);
            int pageSize = 15;
            ViewBag.TotalBook = book.Count();
            return View(book.ToPagedList(pageNumber, pageSize));
        }
        #endregion

        #region Get Today Booking List
        [Authorize(Roles = "admin, superuser")]
        public ActionResult TodayBook(string searchString, string currentFilter, int? page)
        {
            var book = BookListService.TodayBookingList();
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                book = BookListService.SearchTodayBookingList(searchString);
            }
            ViewBag.TotalBook = book.Count();
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(book.ToPagedList(pageNumber, pageSize));
        }
        #endregion

        #region Select Doctor
        public ActionResult Select()
        {
            VM_schedules schedule = new VM_schedules();
            schedule.doctors = DoctorWorkDay.ListWorkDays(); // panggil service hari kerja dokter
            //schedule = schedule.doctors.Where(e => e.dayIn.Count > 0 && e.doctorSpecialties.Count > 0);
            return View(schedule);
        }
        #endregion

        #region HttpGet Create Booking
        public ActionResult Create(string id)
        {
            VM_schedules schedule = new VM_schedules();
            ViewBag.photo = schedule.photo;
            schedule.doctors = new List<VM_doctorList>()
            {
                new VM_doctorList { doctorId = id.ToString() }
            };
            var blockDate = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.doctorId = id;
            ViewBag.blockDate = blockDate;
            return View(schedule);
        }
        #endregion

        #region HttpPost Create Booking
        [HttpPost]
        public ActionResult Create(VM_schedules data)
        {
            
            //var checkUpload = Request.Files.Count;
            if (!ModelState.IsValid)
            {
                return View(data);
            }
            string docId = data.doctors[0].doctorId; // Choosen doctor id
            var checkSchedule = BookListService.CheckSchedule(data.dateSchedule , docId); // Check schedule service
            if (checkSchedule > 0)
            {
                var checkPatient = BookListService.CheckPatient(data.name , data.phone); // check patient service
                if (checkPatient != null) 
                {
                    var getScheduleByToday = db.Schedules.Where(e => e.PatientID == checkPatient.id).ToList();
                    if (CheckPatientScheduleByToday(getScheduleByToday, data.dateSchedule))
                    {
                        TempData["Error"] = "Error";
                        return RedirectToAction("formResult");
                    }
                    else
                    {
                        var dataBooking = BookListService.CreateBooking(checkPatient.id, docId, data.dateSchedule); // Create booking service
                        TempData["id"] = dataBooking;
                        TempData["Error"] = "";
                        return RedirectToAction("formResult");
                    }
                }
                else
                {
                    var type = "";
                    var dataPatient = patientService.CreatePatient(data); // Create patient service
                    if (data.photo != null)
                    {
                        data.photo = data.photo.Replace("data:image/png;base64,", "");
                        string base64 = data.photo.Substring(data.photo.IndexOf(',') + 1);
                        base64 = base64.Trim('\0');

                        byte[] chartData = Convert.FromBase64String(base64);
                        data.photo = data.name.ToString() + ".png";
                        type = "image/png";
                        photoService.UploadPatientPicture(chartData, type, dataPatient.ID);
                        
                        //type = data.photo.ContentType; // Check image type ryan logic
                        //photoService.UploadPatientPicture(data.photo.InputStream, type, dataPatient.id); // Upload patient pictures to database

                    }
                    else if (data.photo == null)
                    {
                        type = data.photos.ContentType; // Check image type ryan logic
                        photoService.UploadPatientPictureFILE(data.photos.InputStream, type, dataPatient.ID); // Upload patient pictures to database

                    }
                    var dataBooking = BookListService.CreateBooking(dataPatient.ID, docId, data.dateSchedule);  // Create booking service
                    TempData["id"] = dataBooking;
                    TempData["Error"] = "";
                    return RedirectToAction("formResult");
                }
            }
            else
            {
                ModelState.AddModelError("dateSchedule", "Doctor is not available on that day");
                var blockDate = DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.blockDate = blockDate;
                return View(data);
            }
        }

        private void CreateImage(byte[] chartData, string v)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Cancel Today Status Booking
        public ActionResult CancelToday(int id)
        {
            var bookingInfo = db.Schedules.First(s => s.ID == id);
            bookingInfo.BookingStatus = "Cancelled";
            db.SaveChanges();
            return RedirectToAction("TodayBook");
        }

        #endregion

        #region Cancel Status Booking
        public ActionResult CancelIndex(int id)
        {
            var bookingInfo = db.Schedules.First(s => s.ID == id);
            bookingInfo.BookingStatus = "Cancelled";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion

        [HttpGet]
        public ActionResult LivePhoto(int id)
        {
            ViewBag.doctorId = id;
            return View();
        }

      
        #region Sign get
        public ActionResult Sign(string id)
        {
            var model = id;
            return View(model);
        }


        #endregion

        #region SignIn HttpGet

        public ActionResult SignIn(string id)
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
            var checkSchedule = BookListService.CheckSchedule(sign.dateSchedule, sign.doctorId.ToString()); // Check schedule service
            if(checkSchedule > 0)
            {
                var checkPatient = BookListService.CheckPatientById(sign.PatientNumber); // check patient service
                if (checkPatient != null)
                {
                    var getScheduleByToday = db.Schedules.Where(e => e.PatientID == checkPatient.id).ToList();
                    if (CheckPatientScheduleByToday(getScheduleByToday,sign.dateSchedule))
                    {
                        ModelState.AddModelError("PatientNumber", "Limit Booking, You Already Booking for That Day!");
                        return View(sign);
                    }
                    else
                    {
                        var dataBooking = BookListService.CreateBooking(sign.PatientNumber, sign.doctorId.ToString(), sign.dateSchedule); // Create booking service
                        TempData["id"] = dataBooking;
                        TempData.Keep();
                        return RedirectToAction("formResult");
                    }
                }
                else
                {
                    ModelState.AddModelError("PatientNumber", "Your Patient Number is wrong");
                    return View(sign);
                }
            }
            else
            {
                ModelState.AddModelError("dateSchedule", "Doctor is not available on that day");
                var blockDate = DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.blockDate = blockDate;
                return View(sign);
            }
        }

        #endregion

        #region FormResult HttpGet

        public ActionResult formResult()
        {
            var Error = "";
            if (TempData.ContainsKey("Error"))
                Error = TempData["Error"].ToString();

            var Status = "";
            if (Error == "Error")
            {
                VM_bookList NewVM = new VM_bookList();
                //NewVM.BookingStatus = "CANCELED";
                //return View(NewVM);
                ViewBag.Error = "true";
                return View(NewVM);
            }
            else
            {
                int id = (int)TempData["id"];
                TempData.Keep();
                var bookingResult = BookListService.BookingListById(id);
                ViewBag.Error = "";
                return View(bookingResult);
            }
        }

        #endregion

        private bool CheckPatientScheduleByToday(List<Schedule> GetData,DateTime GetDateByUserInput)
        {
            bool result = false;

            foreach (var item in GetData)
            {
                if(item.DateSchedule == GetDateByUserInput)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}