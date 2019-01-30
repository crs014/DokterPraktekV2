using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DokterPraktekV2.Models;
namespace DokterPraktekV2.Services
{
    public class SpecialtyService
    {
        private DokterPraktekEntities db = new DokterPraktekEntities();
        public List<SpecialtyData> GetAllSpecialty()
        {
            var ListSpecialties = (from s in db.Specialists
                                   select new SpecialtyData()
                                   {
                                       id = s.ID,
                                       specialty = s.SpecialistName
                                   }).ToList();
            return ListSpecialties;
        }
        public List<doctorSpecialties> GetAllSpecialtyByDocId(string id)
        {
            var ListDoctorSpecialties = (from s in db.Specialists
                                         where s.DoctorSpecialists.Any(x => x.DoctorID == id.ToString())
                                         select new doctorSpecialties
                                         {
                                             id = s.ID,
                                             specialty = s.SpecialistName
                                         }).ToList();
            return ListDoctorSpecialties;
        }
    }
}