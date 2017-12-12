using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Models
{
    public class VM_Specialties
    {
        public List <doctorSpecialties> ListDoctorSpecialties { get; set; }
        public List<SpecialtyData> ListSpecialties { get; set; }
        public int SelectedId { get; set; }
    }

    public class doctorSpecialties
    {
        public int id { get; set; }
        public string specialty { get; set; }
    }

    public class SpecialtyData
    {
        public int id { get; set; }
        public string specialty { get; set; }
    }
    
}