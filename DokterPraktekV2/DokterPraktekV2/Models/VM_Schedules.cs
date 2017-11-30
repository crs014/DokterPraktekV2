using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Models
{
    public class VM_Schedules
    {
        [Required]
        public string name { get; set; }
        [Required]
        public string homeAddress { get; set; }
        [Required]
        public string phone { get; set; }
        [Required]
        public bool gender { get; set; }
        public HttpPostedFile photo { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime dateSchedule { get; set; }
        public List<doctorList> doctors { get; set; }
    }

    public class doctorList
    {
        public int doctorId { get; set; }
        public string name { get; set; }
        public List<workDays> dayIn { get; set; }
    }

    public class workDays
    {
        public string day { get; set; }
    }
}