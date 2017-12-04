using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DokterPraktekV2.Models
{
    public class VM_inputHistory
    {
        public decimal price { get; set; }
        public string sick { get; set; }
        public string descriptionSick { get; set; }
        public int[] medicineId { get; set; }
        public int[] quantity { get; set; }
        public string[] describeMedic { get; set; }
    }
}