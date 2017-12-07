using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace DokterPraktekV2.Models
{
    public class VM_SignIn
    {
        [DataType(DataType.Text)]
        public int PatientNumber { get; set; }
        public int doctorId { get; set; }
        public DateTime dateSchedule { get; set; }
    }
}