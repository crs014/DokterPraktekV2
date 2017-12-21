using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Models
{
    public class VM_schedules
    {
        [DisplayName("Name")]
        [Required]
        public string name { get; set; }
        [DisplayName("Home Address")]
        [Required]
        public string homeAddress { get; set; }
        [DisplayName("Phone Number")]
        [Required]
        public string phone { get; set; }
        [DisplayName("Gender")]
        [Required]
        public bool gender { get; set; }
        public HttpPostedFileBase photo { get; set; }
        [DisplayName("Booking Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime dateSchedule { get; set; }
        public List<VM_doctorList> doctors { get; set; }
    }
}