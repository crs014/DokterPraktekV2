using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DokterPraktekV2.Models;
namespace DokterPraktekV2.Services
{
    public class bookingListServices
    {
        private DokterPraktekEntities db = new DokterPraktekEntities();
        public List<VM_bookList> SearchBookingList(string searchString)
        {
            var bookList = (from data in db.Schedules
                            where data.Patient.Name.Contains(searchString)
                            orderby data.DateSchedule , data.DoctorID , data.BookingNumber
                            select new VM_bookList()
                            {
                                id = data.ID,
                                NoBooking = data.BookingNumber,
                                PatientName = data.Patient.Name,
                                //DoctorName = data.doctor.name,
                                BookDate = data.DateSchedule,
                                BookingStatus = data.BookingStatus,
                            }).ToList();
            return bookList;
        }

        public List<VM_bookList> BookingList()
        {
            var bookList = (from data in db.Schedules
                            orderby data.DateSchedule descending, data.DoctorID, data.BookingNumber
                            select new VM_bookList()
                            {
                                id = data.ID,
                                NoBooking = data.BookingNumber,
                                PatientName = data.Patient.Name,
                                DoctorName = db.doctors.Where(e=>e.userId == data.DoctorID).FirstOrDefault().name,
                                BookDate = data.DateSchedule,
                                BookingStatus = data.BookingStatus,
                            }).ToList();
            return bookList;
        }

        public VM_bookList BookingListById(int id)
        {
            var bookList = (from data in db.Schedules
                            where data.ID == id
                            select new VM_bookList()
                            {
                                id = data.PatientID,
                                NoBooking = data.BookingNumber,
                                PatientName = data.Patient.Name,
                                DoctorName = db.doctors.Where(e=>e.userId == data.DoctorID).FirstOrDefault().name,
                                BookDate = data.DateSchedule,
                                BookingStatus = data.BookingStatus,
                            }).FirstOrDefault();
            return bookList;
        }

        public List<VM_bookList> TodayBookingList()
        {
            var todayDate = DateTime.Today;
            var bookList = (from data in db.Schedules
                            where data.DateSchedule == todayDate
                            orderby data.DateSchedule, data.DoctorID, data.BookingNumber
                            select new VM_bookList()
                            {
                                id = data.ID,
                                NoBooking = data.BookingNumber,
                                PatientName = data.Patient.Name,
                                //DoctorName = data.doctor.name,
                                BookDate = data.DateSchedule,
                                BookingStatus = data.BookingStatus,
                            }).ToList();
            return bookList;
        }

        public List<VM_bookList> SearchTodayBookingList(string searchString)
        {
            var todayDate = DateTime.Today;
            var bookList = (from data in db.Schedules
                            where data.Patient.Name.Contains(searchString) && data.DateSchedule == todayDate
                            orderby data.DateSchedule, data.DoctorID, data.BookingNumber
                            select new VM_bookList()
                            {
                                id = data.ID,
                                NoBooking = data.BookingNumber,
                                PatientName = data.Patient.Name,
                                //DoctorName = data.doctor.name,
                                BookDate = data.DateSchedule,
                                BookingStatus = data.BookingStatus,
                            }).ToList();
            return bookList;
        }

        public VM_patient CheckPatient(string name , string phoneNumber)
        {
            var checkPatient = (from data in db.Patients
                                where data.Name == name && data.PhoneNumber == phoneNumber
                                select new VM_patient()
                                {
                                    id = data.ID,
                                    name = data.Name,
                                    address = data.Address,
                                    phone = data.PhoneNumber,
                                    gender = data.Gender,
                                    photo = data.Photo,
                                    dateTime = data.CreatedDate
                                }).FirstOrDefault(); ;
            //var checkPatient = db.patients.Where(s => s.name == name && s.phone == phoneNumber).Select(s => s.id);
            return checkPatient;
        }

        public VM_patient CheckPatientById(int id)
        {
            var checkPatient = (from data in db.Patients
                                where data.ID == id
                                select new VM_patient()
                                {
                                    id = data.ID,
                                    name = data.Name,
                                    address = data.Address,
                                    phone = data.PhoneNumber,
                                    gender = data.Gender,
                                    photo = data.Photo,
                                    dateTime = data.CreatedDate
                                }).FirstOrDefault(); ;
            return checkPatient;
        }


        public int CreateBooking(int id , string docId , DateTime dateSchedule)
        {
            var checkLastNumber = (from book in db.Schedules
                                   where book.DoctorID == docId.ToString() && book.DateSchedule == dateSchedule
                                   orderby book.ID descending
                                   select book.BookingNumber).FirstOrDefault();
            if(checkLastNumber == null)
            {
                checkLastNumber = 101;
            }
            else
            {
                checkLastNumber += 1;
            }
            var dataSchedule = new Schedule()
            {
                PatientID = id,
                DoctorID = docId.ToString(),
                DateSchedule = dateSchedule,
                BookingStatus = "Booking",
                BookingNumber = checkLastNumber
            };
            db.Schedules.Add(dataSchedule);
            db.SaveChanges();
            return dataSchedule.ID;
        }

        public int CheckSchedule(DateTime data , string docId)
        {
            var dayChoosen = data.DayOfWeek.ToString(); // Hari yang dipilih

            var checkSchedule = (from works in db.WorkSchedules
                                 where works.DoctorID == docId.ToString() && works.Day == dayChoosen && works.IsAvailable == true
                                 select works).Count();
            return checkSchedule;
        }

    }
}