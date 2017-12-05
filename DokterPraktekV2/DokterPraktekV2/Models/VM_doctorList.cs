using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Models
{
    public class VM_doctorList
    {
            public int doctorId { get; set; }
            public string name { get; set; }
            public List<workDays> dayIn { get; set; }
    }
    public class workDays
    {
        public string day { get; set; }
        public bool isSelected { get; set; }
    }
}