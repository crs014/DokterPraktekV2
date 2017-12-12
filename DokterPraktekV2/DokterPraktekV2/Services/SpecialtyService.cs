using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DokterPraktekV2.Models;
namespace DokterPraktekV2.Services
{
    public class SpecialtyService
    {
        private DokterPraktekEntities1 db = new DokterPraktekEntities1();
        public List<SpecialtyData> GetAllSpecialty()
        {
            var ListSpecialties = (from s in db.specialists
                                   select new SpecialtyData()
                                   {
                                       id = s.id,
                                       specialty = s.specialty
                                   }).ToList();
            return ListSpecialties;
        }
        public List<doctorSpecialties> GetAllSpecialtyByDocId(int id)
        {
            var ListDoctorSpecialties = (from s in db.specialists
                                         where s.doctorSpecialists.Any(x => x.doctorId == id)
                                         select new doctorSpecialties
                                         {
                                             id = s.id,
                                             specialty = s.specialty
                                         }).ToList();
            return ListDoctorSpecialties;
        }
    }
}