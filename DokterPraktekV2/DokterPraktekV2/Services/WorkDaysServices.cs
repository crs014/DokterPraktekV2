    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DokterPraktekV2.Models;
namespace DokterPraktekV2.Services
{
    public class WorkDaysServices
    {
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();
        public List<doctorList>ListWorkDays()
        {
            var doctors = (from doc in db.doctors
                           select new doctorList()
                           {
                               doctorId = doc.id,
                               name = doc.name,
                               dayIn = (from work in db.workDays
                                        where work.doctorId == doc.id
                                        select new workDays()
                                        {
                                            day = work.dayIn
                                        }).ToList()
                           }).ToList();
            return doctors;
        }
    }
}