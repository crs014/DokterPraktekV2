using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Services
{
    public class DoctorService
    {
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();

        /* get doctor data from userID*/
        public doctor DoctorAuth(string userId)
        {
            doctor data = db.doctors.FirstOrDefault(e => e.userId == userId);
            return data;
        }

        /* check today schedule from column doctorId*/
        public List<schedule> TodaySchedule(int id)
        {
            var data = db.schedules.Where(
                e => e.doctorId == id && e.dateSchedule == DateTime.Today.Date
                ).ToList();
            return data;
        }
        

        /* get medicine from column doctorId*/
        public List<medicine> getDoctorMedicine(int id)
        {
            var data = db.medicines.Where(e => e.doctorId == id).ToList();
            return data;
        }
        
        
        /* get doctor detail from column id*/               
        public doctor doctorDetail(int id)
        {
            var data = db.doctors.Where(e => e.id == id).First();
            return data;
        }
        
        /* get detail schedule from column id*/
        public schedule scheduleDetail(int id)
        {
            var data = db.schedules.FirstOrDefault(e => e.id == id);
            return data;
        }

        // Create default working days in database
        public void createDays(int id)
        {
            List<string> days = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
            foreach (var item in days)
            {
                var dayIn = new workDay()
                {
                    dayIn = item,
                    doctorId = id,
                    working = false
                };
                db.workDays.Add(dayIn);
                db.SaveChanges();
            }

        }
    }
}