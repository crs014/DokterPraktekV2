using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Services
{
    public class DoctorService
    {
        private DokterPraktekEntities db = new DokterPraktekEntities();

        /* get doctor data from userID*/
        public doctor DoctorAuth(string userId)
        {
            doctor data = db.doctors.FirstOrDefault(e => e.userId == userId);
            return data;
        }

        /* check today schedule from column doctorId*/
        public List<Schedule> TodaySchedule(string id)
        {
            var data = db.Schedules.Where(
                e => e.DoctorID == id.ToString() && e.DateSchedule == DateTime.Today.Date && e.BookingStatus != "Completed"
                ).ToList();
            return data;
        }
        

        /* get medicine from column doctorId*/
        public List<Medicine> getDoctorMedicine(int id)
        {
            var data = db.Medicines.Where(e => e.DoctorID == id.ToString()).ToList();
            return data;
        }
        
        
        /* get doctor detail from column id*/               
        public doctor doctorDetail(int id)
        {
            var data = db.doctors.Where(e => e.id == id).First();
            return data;
        }
        
        /* get detail schedule from column id*/
        public Schedule scheduleDetail(string id,int patientId)
        {
            var data = db.Schedules.FirstOrDefault(e => e.DoctorID == id && e.PatientID == patientId);
            return data;
        }
        public Schedule scheduleDetail(string id)
        {
            var data = db.Schedules.FirstOrDefault(e => e.DoctorID == id );
            return data;
        }

        // Create default working days in database
        public void createDays(int id)
        {
            List<string> days = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
            foreach (var item in days)
            {
                var dayIn = new WorkSchedule()
                {
                    Day = item,
                    DoctorID = id.ToString(),
                    IsAvailable = false
                };
                db.WorkSchedules.Add(dayIn);
                db.SaveChanges();
            }

        }
    }
}