using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Models
{
    public class VM_patient
    {
        public int id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public bool gender { get; set; }
        public string photo { get; set; }
        public byte[] dateTime { get; set; }
    }
}