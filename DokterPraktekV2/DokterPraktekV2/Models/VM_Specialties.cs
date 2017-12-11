using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Models
{
    public class VM_Specialties
    {
        public List<SpecialtyData> ListSpecialties { get; set; }
        public int[] SelectedId { get; set; }
    }

    public class SpecialtyData
    {
        public int id { get; set; }
        public string specialty { get; set; }
    }
    
}