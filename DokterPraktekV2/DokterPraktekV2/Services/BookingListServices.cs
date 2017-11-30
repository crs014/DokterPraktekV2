using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DokterPraktekV2.Models;
namespace DokterPraktekV2.Services
{
    public class BookingListServices
    {
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();
        public List<VM_BookList> BookingList()
        {
            var bookList = (from data in db.schedules
                            orderby data.dateSchedule
                            select new VM_BookList()
                            {
                                NoBooking = data.id,
                                PatientName = data.patient.name,
                                DoctorName = data.doctor.name,
                                BookDate = data.dateSchedule,
                                BookingStatus = data.bookingStatus,
                            }).ToList();
            return bookList;
        }

        public List<VM_BookList> TodayBookingList()
        {
            var todayDate = DateTime.Today;
            var bookList = (from data in db.schedules
                            where data.dateSchedule == todayDate
                            orderby data.id
                            select new VM_BookList()
                            {
                                NoBooking = data.id,
                                PatientName = data.patient.name,
                                DoctorName = data.doctor.name,
                                BookDate = data.dateSchedule,
                                BookingStatus = data.bookingStatus,
                            }).ToList();
            return bookList;
        }

    }
}