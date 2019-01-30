using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DokterPraktekV2.Models;
namespace DokterPraktekV2.Services
{
    public class dayInServices
    {
        private DokterPraktekEntities db = new DokterPraktekEntities();
        
        public docInfo DoctorNames(string id)
        {
            var docNames = (from doc in db.doctors
                            where doc.id.ToString() == id
                            select new docInfo()
                            {
                                doctorId = doc.id,
                                doctorName = doc.name,
                            }).FirstOrDefault();

            return docNames;
        }

        public List<dayIn> DoctorDayIn(string id)
        {
            var docDays = (from doc in db.WorkSchedules
                            where doc.DoctorID == id.ToString()
                            select new dayIn()
                            {
                                dayId = doc.ID,
                                day = doc.Day,
                                IsSelected = doc.IsAvailable,
                            }).ToList();

            return docDays;
        }

        public List<VM_doctorList>ListWorkDays()
        {
            
            var doctors = (from doc in db.doctors
                           select new VM_doctorList()
                           {
                               doctorId = doc.userId,
                               name = doc.name,
                               dayIn = (from work in db.WorkSchedules
                                        where work.DoctorID == doc.userId && work.IsAvailable == true
                                        select new workDays()
                                        {
                                            day = work.Day
                                        }).ToList(),
                               doctorSpecialties = (from s in db.Specialists
                                                    where s.DoctorSpecialists.Any(x=>x.DoctorID == doc.userId.ToString())
                                                    select new docSpecialist
                                                    {
                                                        specialty = s.SpecialistName
                                                    }).ToList()
                           }).ToList();
            return doctors;
        }
    }
}