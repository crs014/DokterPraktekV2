using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Models
{
    public class VM_patientMedicine
    {
        public int id { get; set; }
        public int historyId { get; set; }
        public string medicineName { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public string description { get; set;}
       
    }
}