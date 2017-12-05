using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Models
{
    public class VM_schedules
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
        public List<VM_doctorList> doctors { get; set; }
    }
}