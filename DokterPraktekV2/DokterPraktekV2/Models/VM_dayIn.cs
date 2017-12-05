using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace DokterPraktekV2.Models
{
    public class VM_dayIn
    {
        public int docId { get; set; }
        public docInfo docInfo { get; set; }
        public List<dayIn> dayIn { get; set; }
    }
    public class docInfo
    {
        public int doctorId { get; set; }
        public string doctorName { get; set; }

    }
    public class dayIn
    {
        public int dayId { get; set; }
        public string day { get; set; }
        public bool IsSelected { get; set; }
    }
}
