using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Models
{
    public class VM_history
    {
        public int id { get; set; }
        public string doctorId { get; set; }
        public string doctorName { get; set; }
        public int patientId { get; set; }
        public string patientName { get; set; }
        public string sickness { get; set; }
        public string description { get; set; }
        public System.DateTime date { get; set; }
        public decimal checkupPrice { get; set; }
        public bool gender { get; set; }      
    }
}