using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Models
{
    public class VM_bookList
    {
        public int NoBooking { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime BookDate { get; set; }
        public string BookingStatus { get; set; }
    }
    
}