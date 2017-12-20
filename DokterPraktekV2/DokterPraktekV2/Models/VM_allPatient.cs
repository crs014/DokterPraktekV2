using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Models
{
    public class VM_allPatient
    {
        public string patientName { get; set; }
        public string patientAddress { get; set; }
        public string patientPhoneNumber { get; set; }
        public bool gender { get; set; }
        public byte[] bytes { get; set; }
        public string mime { get; set; }

    }
}