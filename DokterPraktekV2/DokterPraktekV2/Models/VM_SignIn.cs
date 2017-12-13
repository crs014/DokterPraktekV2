using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace DokterPraktekV2.Models
{
    public class VM_SignIn
    {
        [DisplayName("Patient Number")]
        [DataType(DataType.Text)]
        public int PatientNumber { get; set; }
        public int doctorId { get; set; }
        [DisplayName("Booking Date")]
        public DateTime dateSchedule { get; set; }
    }
}