using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Services
{
    public class DoctorService
    {
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();

        /* check today schedule from column doctorId*/
        public List<schedule> TodaySchedule(int id)
        {
            var data = db.schedules.Where(
                e => e.doctorId == id && e.dateSchedule == DateTime.Today.Date
                ).ToList();
            return data;
        }
        
        public doctor doctorDetail(int id)
        {
            var data = db.doctors.Where(e => e.id == id).First();
            return data;
        }  
    }
}