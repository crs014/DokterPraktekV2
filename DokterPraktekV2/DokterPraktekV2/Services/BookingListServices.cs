using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DokterPraktekV2.Models;
namespace DokterPraktekV2.Services
{
    public class bookingListServices
    {
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();
        public List<VM_bookList> BookingList()
        {
            var bookList = (from data in db.schedules
                            orderby data.dateSchedule , data.doctorId , data.bookingNumber
                            select new VM_bookList()
                            {
                                NoBooking = data.bookingNumber,
                                PatientName = data.patient.name,
                                DoctorName = data.doctor.name,
                                BookDate = data.dateSchedule,
                                BookingStatus = data.bookingStatus,
                            }).ToList();
            return bookList;
        }

        public VM_bookList BookingListById(int id)
        {
            var bookList = (from data in db.schedules
                            where data.id == id
                            select new VM_bookList()
                            {
                                id = data.id,
                                NoBooking = data.bookingNumber,
                                PatientName = data.patient.name,
                                DoctorName = data.doctor.name,
                                BookDate = data.dateSchedule,
                                BookingStatus = data.bookingStatus,
                            }).FirstOrDefault();
            return bookList;
        }

        public List<VM_bookList> TodayBookingList()
        {
            var todayDate = DateTime.Today;
            var bookList = (from data in db.schedules
                            where data.dateSchedule == todayDate
                            orderby data.id
                            select new VM_bookList()
                            {
                                NoBooking = data.id,
                                PatientName = data.patient.name,
                                DoctorName = data.doctor.name,
                                BookDate = data.dateSchedule,
                                BookingStatus = data.bookingStatus,
                            }).ToList();
            return bookList;
        }

        public VM_patient CheckPatient(string name , string phoneNumber)
        {
            var checkPatient = (from data in db.patients
                                where data.name == name && data.phone == phoneNumber
                                select new VM_patient()
                                {
                                    id = data.id,
                                    name = data.name,
                                    address = data.homeAddress,
                                    phone = data.phone,
                                    gender = data.gender,
                                    photo = data.photo,
                                    dateTime = data.registerDatetime
                                }).FirstOrDefault(); ;
            //var checkPatient = db.patients.Where(s => s.name == name && s.phone == phoneNumber).Select(s => s.id);
            return checkPatient;
        }

        public VM_patient CheckPatientById(int id)
        {
            var checkPatient = (from data in db.patients
                                where data.id == id
                                select new VM_patient()
                                {
                                    id = data.id,
                                    name = data.name,
                                    address = data.homeAddress,
                                    phone = data.phone,
                                    gender = data.gender,
                                    photo = data.photo,
                                    dateTime = data.registerDatetime
                                }).FirstOrDefault(); ;
            return checkPatient;
        }


        public int CreateBooking(int id , int docId , DateTime dateSchedule)
        {
            var checkLastNumber = (from book in db.schedules
                                   where book.doctorId == docId && book.dateSchedule == dateSchedule
                                   orderby book.id descending
                                   select book.bookingNumber).FirstOrDefault();
            if(checkLastNumber == null)
            {
                checkLastNumber = 101;
            }
            else
            {
                checkLastNumber += 1;
            }
            var dataSchedule = new schedule()
            {
                patientId = id,
                doctorId = docId,
                dateSchedule = dateSchedule,
                bookingStatus = "Booking",
                bookingNumber = checkLastNumber
            };
            db.schedules.Add(dataSchedule);
            db.SaveChanges();
            return dataSchedule.id;
        }

        public int CheckSchedule(VM_schedules data , int docId)
        {
            var dayChoosen = data.dateSchedule.DayOfWeek.ToString(); // Hari yang dipilih

            var checkSchedule = (from works in db.workDays
                                 where works.doctorId == docId && works.dayIn == dayChoosen && works.working == true
                                 select works).Count();
            return checkSchedule;
        }

    }
}