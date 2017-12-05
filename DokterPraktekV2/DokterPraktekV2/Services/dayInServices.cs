    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DokterPraktekV2.Models;
namespace DokterPraktekV2.Services
{
    public class dayInServices
    {
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();
        //
        public docInfo DoctorNames(int id)
        {
            var docNames = (from doc in db.doctors
                            where doc.id == id
                            select new docInfo()
                            {
                                doctorId = doc.id,
                                doctorName = doc.name,
                            }).FirstOrDefault();

            return docNames;
        }

        public List<dayIn> DoctorDayIn(int id)
        {
            var docDays = (from doc in db.workDays
                            where doc.doctorId == id
                            select new dayIn()
                            {
                                dayId = doc.id,
                                day = doc.dayIn,
                                IsSelected = doc.working,
                            }).ToList();

            return docDays;
        }

        public List<VM_doctorList>ListWorkDays()
        {
            var doctors = (from doc in db.doctors
                           select new VM_doctorList()
                           {
                               doctorId = doc.id,
                               name = doc.name,
                               dayIn = (from work in db.workDays
                                        where work.doctorId == doc.id && work.working == true
                                        select new workDays()
                                        {
                                            day = work.dayIn
                                        }).ToList()
                           }).ToList();
            return doctors;
        }
    }
}